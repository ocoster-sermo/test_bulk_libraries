using Microsoft.EntityFrameworkCore;

namespace TestBulkDbLibraries.Entities;

public class Employee : EntityBase
{
    public int? ManagerId { get; set; }
    public Manager Manager { get; set; } = default!;
    public int? LocationId { get; set; }
    public int? BuildingId { get; set; }
    public int? OriginalBuildingId { get; set; }
    public int? ProviderId { get; set; }
    public int? ProviderSettingsId { get; set; }
    public EmployeeStatus Status { get; set; }
    public bool EligibleForBonus { get; set; }

    public override void Map(ModelBuilder modelBuilder)
    {
        modelBuilder
            .Entity<Employee>();
    }
}
