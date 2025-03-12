using System.Globalization;
using System.Reflection;
using Microsoft.EntityFrameworkCore;
using TestBulkDbLibraries.Entities;

namespace TestBulkDbLibraries.Database;

public class DatabaseContext : DbContext, IDatabaseContext
{
    public DatabaseContext(DbContextOptions databaseContextOptions)
    : base(databaseContextOptions)
    {
        AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        ProcessEntityMappers(modelBuilder);

        base.OnModelCreating(modelBuilder);
    }

    protected virtual void ProcessEntityMappers(ModelBuilder modelBuilder)
    {
        // Get the type of the IEntityMapper interface.
        var type = typeof(IMappedEntity);

        // Get all the classes we have that implements the IEntityMapper interface in this assembly.
        var types = type.GetTypeInfo().Assembly.GetTypes().Where(p => type.IsAssignableFrom(p));

        // Loop through all the classes.
        foreach (var t in types)
        {
            // No point in trying to instantiate abstract classes.
            if (!t.GetTypeInfo().IsAbstract)
            {
                // Create a new instance of each class.
                if (Activator.CreateInstance(t) is IMappedEntity instance)
                {
                    // Invoke the Map method on the instance to allow the modelBuilder to apply the class's mappings.
                    instance.Map(modelBuilder);

                    // Add the XMIN property to enable optimistic concurrency. This is the standard EF core way to mark a property as a concurrency token.
                    modelBuilder.Entity(t)
                        .Property(nameof(IMappedEntity.Xmin))
                        .HasColumnName(nameof(IMappedEntity.Xmin).ToLower(CultureInfo.InvariantCulture))
                        .IsRowVersion();
                }
            }
        }
    }

}
