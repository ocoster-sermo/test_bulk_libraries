using Microsoft.EntityFrameworkCore;

namespace TestBulkDbLibraries.Database;

public interface IDatabaseContextFactory
{
    IDatabaseContext Create(bool readOnly = false);
}

public interface IDatabaseContext : IDisposable
{
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);

    DbSet<TEntity> Set<TEntity>() where TEntity : class;
}
