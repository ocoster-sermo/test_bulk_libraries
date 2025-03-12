using Microsoft.EntityFrameworkCore;

namespace TestBulkDbLibraries.Entities;

public interface IMappedEntity
{
    /// <summary>
    /// EF core uses this to handle concurrency.<br />
    /// With npgsql, this translates to using the XMIN column, as configured in the DatabaseContext.
    /// </summary>
    uint Xmin { get; set; }

    void Map(ModelBuilder modelBuilder);
}
