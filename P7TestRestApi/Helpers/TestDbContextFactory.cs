using Microsoft.EntityFrameworkCore;
using P7CreateRestApi.Data;

namespace P7TestRestApi.Helpers;

public static class TestDbContextFactory
{
    public static LocalDbContext Create(string? dbName = null)
    {
        var options = new DbContextOptionsBuilder<LocalDbContext>()
            .UseInMemoryDatabase(databaseName: dbName ?? Guid.NewGuid().ToString("N"))
            .EnableSensitiveDataLogging()
            .Options;

        var ctx = new LocalDbContext(options);
        ctx.Database.EnsureCreated();
        return ctx;
    }
}
