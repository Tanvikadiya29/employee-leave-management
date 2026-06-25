namespace LeaveManagement.Tests.Services;

public class EmployeeServiceTests
{
    private static (AppDbContext ctx, EmployeeService svc) Build(string name)
    {
        var ctx = DbContextFactory.CreateWithSeed(name);
        return (ctx, new EmployeeService(ctx));
    }

    [Fact]
    public async Task GetAllAsync_ReturnsOnlyActiveEmployees()
    {
        var (_, svc) = Build(nameof(GetAllAsync_ReturnsOnlyActiveEmployees));

        // Seed has 2 active employees (Jane, Alice) + 1 inactive (Bob)
        var result = await svc.GetAllAsync(new PaginationParams());

        result.Success.Should().BeTrue();
        result.Data!.Should().HaveCount(2, "inactive users must be excluded");
        result.Data.Should().OnlyContain(e => e.IsActive);
    }

    [Fact]
    public async Task GetByIdAsync_ActiveEmployee_ReturnsEmployee()
    {
        var (_, svc) = Build(nameof(GetByIdAsync_ActiveEmployee_ReturnsEmployee));

        var result = await svc.GetByIdAsync(SeedData.ActiveEmployeeId);

        result.Success.Should().BeTrue();
        result.Data!.Id.Should().Be(SeedData.ActiveEmployeeId);
        result.Data.Email.Should().Be("jane@test.com");
        result.Data.FullName.Should().Be("Jane Smith");
    }

    [Fact]
    public async Task GetByIdAsync_NonExistentId_ReturnsError()
    {
        var (_, svc) = Build(nameof(GetByIdAsync_NonExistentId_ReturnsError));

        var result = await svc.GetByIdAsync(9999);

        result.Success.Should().BeFalse();
        result.Message.Should().ContainEquivalentOf("not found");
    }

    [Fact]
    public async Task CreateAsync_ValidEmployee_Succeeds()
    {
        var (ctx, svc) = Build(nameof(CreateAsync_ValidEmployee_Succeeds));
        int before = ctx.Users.Count(u => u.RoleId == SeedData.EmployeeRoleId);

        var result = await svc.CreateAsync(new CreateEmployeeDto
        {
            FirstName   = "New",
            LastName    = "Hire",
            Email       = "newhire@company.com",
            Password    = "NewHire@123",
            Department  = "Engineering",
            Designation = "Software Engineer"
        });

        result.Success.Should().BeTrue();
        result.Data!.Email.Should().Be("newhire@company.com");
        result.Data.RoleName.Should().Be("Employee");
        result.Data.IsActive.Should().BeTrue();
        ctx.Users.Count(u => u.RoleId == SeedData.EmployeeRoleId).Should().Be(before + 1);
    }

    [Fact]
    public async Task CreateAsync_DuplicateEmail_Fails()
    {
        // Rule #12
        var (_, svc) = Build(nameof(CreateAsync_DuplicateEmail_Fails));

        var result = await svc.CreateAsync(new CreateEmployeeDto
        {
            FirstName = "Dup",
            LastName  = "Email",
            Email     = "jane@test.com", // already exists
            Password  = "DupEmail@1"
        });

        result.Success.Should().BeFalse();
        result.Message.Should().ContainEquivalentOf("email");
    }

    [Fact]
    public async Task UpdateAsync_PartialUpdate_OnlyChangesProvidedFields()
    {
        var (ctx, svc) = Build(nameof(UpdateAsync_PartialUpdate_OnlyChangesProvidedFields));
        var originalEmail = ctx.Users.Find(SeedData.ActiveEmployeeId)!.Email;

        var result = await svc.UpdateAsync(SeedData.ActiveEmployeeId, new UpdateEmployeeDto
        {
            Department  = "Product",
            Designation = "Senior Engineer"
        });

        result.Success.Should().BeTrue();
        result.Data!.Department.Should().Be("Product");
        result.Data.Designation.Should().Be("Senior Engineer");
        var dbUser = ctx.Users.Find(SeedData.ActiveEmployeeId)!;
        dbUser.Email.Should().Be(originalEmail, "email must not be altered by update");
    }

    [Fact]
    public async Task UpdateAsync_DeactivateEmployee_SetsIsActiveFalse()
    {
        var (ctx, svc) = Build(nameof(UpdateAsync_DeactivateEmployee_SetsIsActiveFalse));

        var result = await svc.UpdateAsync(SeedData.ActiveEmployeeId, new UpdateEmployeeDto
        {
            IsActive = false
        });

        result.Success.Should().BeTrue();
        ctx.Users.Find(SeedData.ActiveEmployeeId)!.IsActive.Should().BeFalse();
    }

    [Fact]
    public async Task DeleteAsync_ActiveEmployee_SoftDeletesRecord()
    {
        var (ctx, svc) = Build(nameof(DeleteAsync_ActiveEmployee_SoftDeletesRecord));
        int totalBefore = ctx.Users.Count();

        var result = await svc.DeleteAsync(SeedData.ActiveEmployeeId);

        result.Success.Should().BeTrue();
        ctx.Users.Count().Should().Be(totalBefore, "soft delete must not remove the row");
        ctx.Users.Find(SeedData.ActiveEmployeeId)!.IsActive.Should().BeFalse();
    }

    [Fact]
    public async Task DeleteAsync_NonExistentEmployee_ReturnsError()
    {
        var (_, svc) = Build(nameof(DeleteAsync_NonExistentEmployee_ReturnsError));

        var result = await svc.DeleteAsync(9999);

        result.Success.Should().BeFalse();
        result.Message.Should().ContainEquivalentOf("not found");
    }
}
