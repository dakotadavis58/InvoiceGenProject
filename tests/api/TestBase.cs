using Microsoft.EntityFrameworkCore;
using InvoiceGeneratorAPI.Data;

namespace InvoiceGeneratorAPI.Tests;

public abstract class TestBase : IDisposable
{
    protected readonly ApplicationDbContext Context;

    protected TestBase()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        Context = new ApplicationDbContext(options);
    }

    public void Dispose()
    {
        Context.Database.EnsureDeleted();
        Context.Dispose();
    }
}