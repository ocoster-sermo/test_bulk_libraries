using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace TestBulkDbLibraries.Database;

public class DatabaseContextFactory : IDatabaseContextFactory
{
    private readonly PooledDbContextFactory<DatabaseContext> pooledDbContextFactory;

    /// <summary>
    /// Creates a new instance of this class.
    /// </summary>
    /// <param name="databaseContextOptions">The database context configuration options to use for entity framework.</param>
    public DatabaseContextFactory(DbContextOptions<DatabaseContext> databaseContextOptions)
    {
        pooledDbContextFactory = new PooledDbContextFactory<DatabaseContext>(databaseContextOptions);
    }

    /// <summary>
    /// Creates a new database context.
    /// </summary>
    /// <param name="readOnly">True to create a database context with no change tracking (read-only); false to enable change tracking.</param>
    /// <returns>The new database context to use.</returns>
    public IDatabaseContext Create(bool readOnly = false)
    {
        var context = pooledDbContextFactory.CreateDbContext();
        context.ChangeTracker.AutoDetectChangesEnabled = !readOnly;
        return context;
    }
}
