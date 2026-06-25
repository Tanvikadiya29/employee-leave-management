namespace LeaveManagement.Tests.Services;

public class AuthServiceTests
{
    private static (AppDbContext ctx, AuthService svc) Build(string name)
    {
        var ctx        = DbContextFactory.CreateWithSeed(name);
        var jwtService = new JwtService(JwtConfigFactory.Create());
        return (ctx, new AuthService(ctx, jwtService));
    }

    [Fact]
    public async Task LoginAsync_ValidAdminCredentials_ReturnsTokenWithAdminRole()
    {
        var (_, svc) = Build(nameof(LoginAsync_ValidAdminCredentials_ReturnsTokenWithAdminRole));

        var result = await svc.LoginAsync(new LoginDto
        {
            Email    = "admin@company.com",
            Password = "Admin@123"
        });

        result.Should().NotBeNull();
        result!.Token.Should().NotBeNullOrWhiteSpace();
        result.Role.Should().Be("Admin");
        result.UserId.Should().Be(SeedData.ActiveAdminId);
        result.FullName.Should().Be("System Admin");
        result.ExpiresAt.Should().BeAfter(DateTime.UtcNow);
    }

    [Fact]
    public async Task LoginAsync_ValidEmployeeCredentials_ReturnsTokenWithEmployeeRole()
    {
        var (_, svc) = Build(nameof(LoginAsync_ValidEmployeeCredentials_ReturnsTokenWithEmployeeRole));

        var result = await svc.LoginAsync(new LoginDto
        {
            Email    = "jane@test.com",
            Password = "Employee@1"
        });

        result.Should().NotBeNull();
        result!.Role.Should().Be("Employee");
        result.Email.Should().Be("jane@test.com");
    }

    [Fact]
    public async Task LoginAsync_WrongPassword_ReturnsNull()
    {
        var (_, svc) = Build(nameof(LoginAsync_WrongPassword_ReturnsNull));

        var result = await svc.LoginAsync(new LoginDto
        {
            Email    = "admin@company.com",
            Password = "WrongPassword!"
        });

        result.Should().BeNull("wrong password must not return a token");
    }

    [Fact]
    public async Task LoginAsync_NonExistentEmail_ReturnsNull()
    {
        var (_, svc) = Build(nameof(LoginAsync_NonExistentEmail_ReturnsNull));

        var result = await svc.LoginAsync(new LoginDto
        {
            Email    = "nobody@unknown.com",
            Password = "AnyPassword@1"
        });

        result.Should().BeNull();
    }
}
