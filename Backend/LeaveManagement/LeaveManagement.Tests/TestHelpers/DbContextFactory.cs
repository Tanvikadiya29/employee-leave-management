using Microsoft.EntityFrameworkCore;

namespace LeaveManagement.Tests.TestHelpers;

public static class DbContextFactory
{
    public static AppDbContext Create(string databaseName)
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: databaseName)
            .Options;

        var context = new AppDbContext(options);
        context.Database.EnsureCreated();
        return context;
    }

    public static AppDbContext CreateWithSeed(string databaseName)
    {
        var context = Create(databaseName);
        SeedData.Apply(context);
        return context;
    }
}
