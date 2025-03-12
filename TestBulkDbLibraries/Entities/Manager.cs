using Microsoft.EntityFrameworkCore;

namespace TestBulkDbLibraries.Entities;

public class Manager : EntityBase
{
    public Department Department { get; set; }

    public string? DepartmentalId { get; set; }

    public int? CountryId { get; set; }

    public ICollection<Employee> Employees { get; set; } = [];

    public override void Map(ModelBuilder modelBuilder)
    {
        modelBuilder
            .Entity<Manager>();
    }
}
