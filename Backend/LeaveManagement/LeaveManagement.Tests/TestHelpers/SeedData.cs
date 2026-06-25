namespace LeaveManagement.Tests.TestHelpers;

public static class SeedData
{
    // ── Role IDs (must match AppDbContext.HasData values) ────────────────────
    public const int AdminRoleId    = 1;
    public const int EmployeeRoleId = 2;

    // ── User IDs (high values to avoid collisions with auto-increment) ────────
    public const int ActiveAdminId      = 10;
    public const int ActiveEmployeeId   = 20;
    public const int InactiveEmployeeId = 21;
    public const int SecondEmployeeId   = 22;

    public static void Apply(AppDbContext context)
    {
      
        context.Users.AddRange(
            new User
            {
                Id           = ActiveAdminId,
                FirstName    = "System",
                LastName     = "Admin",
                Email        = "admin@company.com",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("Admin@123", 4),
                RoleId       = AdminRoleId,
                IsActive     = true
            },
            new User
            {
                Id           = ActiveEmployeeId,
                FirstName    = "Jane",
                LastName     = "Smith",
                Email        = "jane@test.com",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("Employee@1", 4),
                RoleId       = EmployeeRoleId,
                IsActive     = true
            },
            new User
            {
                Id           = InactiveEmployeeId,
                FirstName    = "Bob",
                LastName     = "Inactive",
                Email        = "bob@test.com",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("Employee@1", 4),
                RoleId       = EmployeeRoleId,
                IsActive     = false
            },
            new User
            {
                Id           = SecondEmployeeId,
                FirstName    = "Alice",
                LastName     = "Jones",
                Email        = "alice@test.com",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("Employee@1", 4),
                RoleId       = EmployeeRoleId,
                IsActive     = true
            }
        );

        context.SaveChanges();
    }

   
    public static DateOnly FutureDate(int offsetDays = 5)
        => DateOnly.FromDateTime(DateTime.Today.AddDays(offsetDays));
}
