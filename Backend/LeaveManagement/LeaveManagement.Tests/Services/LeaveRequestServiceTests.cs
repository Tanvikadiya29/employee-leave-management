namespace LeaveManagement.Tests.Services;

public class LeaveRequestServiceTests
{
    // ── Helpers ──────────────────────────────────────────────────────────────
    private static (AppDbContext ctx, LeaveRequestService svc) Build(string name)
    {
        var ctx = DbContextFactory.CreateWithSeed(name);
        return (ctx, new LeaveRequestService(ctx));
    }

    private static CreateLeaveRequestDto ValidDto(int fromOffset = 5, int toOffset = 7,
        string reason = "Annual family vacation") => new()
    {
        FromDate = SeedData.FutureDate(fromOffset),
        ToDate   = SeedData.FutureDate(toOffset),
        Reason   = reason
    };


    private static int AddLeave(AppDbContext ctx, int employeeId,
        int fromOffset, int toOffset, LeaveStatus status, string reason = "Test leave")
    {
        var leave = new LeaveRequest
        {
            EmployeeId = employeeId,
            FromDate   = SeedData.FutureDate(fromOffset),
            ToDate     = SeedData.FutureDate(toOffset),
            Reason     = reason,
            Status     = status
        };
        ctx.LeaveRequests.Add(leave);
        ctx.SaveChanges();
        return leave.Id;
    }

    [Fact]
    public async Task CreateAsync_ValidRequest_ReturnsSuccess()
    {
        var (ctx, svc) = Build(nameof(CreateAsync_ValidRequest_ReturnsSuccess));

        var result = await svc.CreateAsync(ValidDto(5, 7), SeedData.ActiveEmployeeId);

        result.Success.Should().BeTrue();
        result.Data.Should().NotBeNull();
        result.Data!.Status.Should().Be("Pending");
        result.Data.EmployeeId.Should().Be(SeedData.ActiveEmployeeId);
        result.Data.TotalDays.Should().Be(3);
        ctx.LeaveRequests.Count().Should().Be(1);
    }

    [Fact]
    public async Task CreateAsync_EmployeeDoesNotExist_ReturnsError()
    {
        var (ctx, svc) = Build(nameof(CreateAsync_EmployeeDoesNotExist_ReturnsError));

        var result = await svc.CreateAsync(ValidDto(), employeeId: 9999);

        result.Success.Should().BeFalse();
        result.Message.Should().ContainEquivalentOf("not found");
        ctx.LeaveRequests.Should().BeEmpty();
    }

    [Fact]
    public async Task CreateAsync_InactiveEmployee_ReturnsError()
    {
        var (ctx, svc) = Build(nameof(CreateAsync_InactiveEmployee_ReturnsError));

        var result = await svc.CreateAsync(ValidDto(), SeedData.InactiveEmployeeId);

        result.Success.Should().BeFalse();
        result.Message.Should().ContainEquivalentOf("inactive");
        ctx.LeaveRequests.Should().BeEmpty();
    }

    [Fact]
    public async Task CreateAsync_ToDateBeforeFromDate_ReturnsError()
    {
        var (_, svc) = Build(nameof(CreateAsync_ToDateBeforeFromDate_ReturnsError));

        var result = await svc.CreateAsync(new CreateLeaveRequestDto
        {
            FromDate = SeedData.FutureDate(10),
            ToDate   = SeedData.FutureDate(5),
            Reason   = "Bad dates"
        }, SeedData.ActiveEmployeeId);

        result.Success.Should().BeFalse();
        result.Message.Should().ContainEquivalentOf("end date");
    }

    [Fact]
    public async Task CreateAsync_FromDateInPast_ReturnsError()
    {
        var (_, svc) = Build(nameof(CreateAsync_FromDateInPast_ReturnsError));

        var result = await svc.CreateAsync(new CreateLeaveRequestDto
        {
            FromDate = DateOnly.FromDateTime(DateTime.Today.AddDays(-1)),
            ToDate   = DateOnly.FromDateTime(DateTime.Today.AddDays(3)),
            Reason   = "Past date leave"
        }, SeedData.ActiveEmployeeId);

        result.Success.Should().BeFalse();
        result.Message.Should().ContainEquivalentOf("past");
    }

    [Fact]
    public async Task CreateAsync_ExactSameDatesAsApproved_IsBlocked()
    {
        var (ctx, svc) = Build(nameof(CreateAsync_ExactSameDatesAsApproved_IsBlocked));
        AddLeave(ctx, SeedData.ActiveEmployeeId, 5, 10, LeaveStatus.Approved);

        var result = await svc.CreateAsync(new CreateLeaveRequestDto
        {
            FromDate = SeedData.FutureDate(5),
            ToDate   = SeedData.FutureDate(10),
            Reason   = "Same dates"
        }, SeedData.ActiveEmployeeId);

        result.Success.Should().BeFalse();
        result.Message.Should().ContainEquivalentOf("overlap");
    }

    [Fact]
    public async Task CreateAsync_NewStartsInsideExisting_IsBlocked()
    {
        var (ctx, svc) = Build(nameof(CreateAsync_NewStartsInsideExisting_IsBlocked));
        AddLeave(ctx, SeedData.ActiveEmployeeId, 5, 15, LeaveStatus.Approved);

        var result = await svc.CreateAsync(new CreateLeaveRequestDto
        {
            FromDate = SeedData.FutureDate(10),
            ToDate   = SeedData.FutureDate(20),
            Reason   = "Partial overlap"
        }, SeedData.ActiveEmployeeId);

        result.Success.Should().BeFalse();
        result.Message.Should().ContainEquivalentOf("overlap");
    }

    [Fact]
    public async Task CreateAsync_OverlappingRejectedLeave_IsAllowed()
    {
        var (ctx, svc) = Build(nameof(CreateAsync_OverlappingRejectedLeave_IsAllowed));
        AddLeave(ctx, SeedData.ActiveEmployeeId, 5, 10, LeaveStatus.Rejected);

        var result = await svc.CreateAsync(new CreateLeaveRequestDto
        {
            FromDate = SeedData.FutureDate(5),
            ToDate   = SeedData.FutureDate(10),
            Reason   = "Retry after rejection"
        }, SeedData.ActiveEmployeeId);

        result.Success.Should().BeTrue("rejected leaves must not block new requests");
    }
}
