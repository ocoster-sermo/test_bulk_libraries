using EFCore.BulkExtensions;
using TestBulkDbLibraries.Database;
using TestBulkDbLibraries.Entities;

namespace TestBulkDbLibraries;

public interface ITester
{
    Task RunBulkImport(List<string> idsToImport, int countryId, int locationId, int buildingId, int originalBuildingId, int providerId, int providerSettingsId, bool bonusEligible, CancellationToken cancellationToken = default);
}

public class Tester : ITester
{
    private readonly IDatabaseContextFactory contextFactory;

    public Tester(IDatabaseContextFactory contextFactory)
    {
        this.contextFactory = contextFactory;
    }

    public async Task RunBulkImport(List<string> idsToImport, int countryId, int locationId, int buildingId, int originalBuildingId, int providerId, int providerSettingsId, bool bonusEligible, CancellationToken cancellationToken = default)
    {
        var allManagers = idsToImport.Select(id => new Manager
        {
            Department = Department.Sales,
            DepartmentalId = id,
            CountryId = countryId,
            Employees =
            [
                new ()
                    {
                        LocationId = locationId,
                        BuildingId = buildingId,
                        OriginalBuildingId = originalBuildingId,
                        ProviderId = providerId,
                        ProviderSettingsId = providerSettingsId,
                        Status = EmployeeStatus.Veteran,
                        EligibleForBonus = bonusEligible,
                    }
            ]
        }).ToList();

        await BulkExtensionsSimpleTransactionAsync(allManagers, cancellationToken);
        //await BulkExtensionsSimpleAsync(allManagers, cancellationToken);
        //await VanillaEfCoreAsync(allManagers, cancellationToken);
    }

    private async Task BulkExtensionsSimpleTransactionAsync(List<Manager> allManagers, CancellationToken cancellationToken)
    {
        /*
         C#:
         PostgresException: 42703: column "Promotions" of relation "employee" does not exist

         DB:
         ERROR:  column "Promotions" of relation "employee" does not exist
         STATEMENT:  COPY "public"."employee" ("building_id", "created_at", "eligible_for_bonus", "location_id", "manager_id", "modified_at", "original_building_id", "provider_id", "provider_settings_id", "status", "Promotions") FROM STDIN (FORMAT BINARY);
         LOG:  could not receive data from client: An existing connection was forcibly closed by the remote host.
         */

        using var context = contextFactory.Create() as DatabaseContext;

        var transaction = context!.Database.BeginTransaction();

        await context!.BulkInsertAsync(allManagers,
                                    new BulkConfig
                                    {
                                        UseUnlogged = true,
                                    },
                        cancellationToken: cancellationToken);

        var firstEmployee = allManagers.First().Employees.First();

        var employees = allManagers.Select(m => new Employee
        {
            ManagerId = m.Id,
            LocationId = firstEmployee.LocationId,
            BuildingId = firstEmployee.BuildingId,
            OriginalBuildingId = firstEmployee.OriginalBuildingId,
            ProviderId = firstEmployee.ProviderId,
            ProviderSettingsId = firstEmployee.ProviderSettingsId,
            Status = firstEmployee.Status,
            EligibleForBonus = firstEmployee.EligibleForBonus,
        }).ToList();

        await context!.BulkInsertAsync(employees,
                            new BulkConfig
                            {
                                UseUnlogged = true,
                            },
                cancellationToken: cancellationToken);

        await transaction.CommitAsync(cancellationToken);
    }

    private async Task BulkExtensionsSimpleAsync(List<Manager> allManagers, CancellationToken cancellationToken)
    {
        /*
         C#:
         PostgresException: 42703: column "Promotions" of relation "employeeTempaa5d0fea" does not exist
        
         DB:
         ERROR:  column "Promotions" of relation "employeeTempaa5d0fea" does not exist
         STATEMENT:  COPY "public"."employeeTempaa5d0fea" ("id", "building_id", "created_at", "eligible_for_bonus", "location_id", "manager_id", "modified_at", "original_building_id", "provider_id", "provider_settings_id", "status", "Promotions") FROM STDIN (FORMAT BINARY);
         ERROR:  current transaction is aborted, commands ignored until end of transaction block
         STATEMENT:  DROP TABLE IF EXISTS "public"."employeeTempaa5d0fea"
         LOG:  could not receive data from client: An existing connection was forcibly closed by the remote host.         
        */

        using var context = contextFactory.Create() as DatabaseContext;

        await context!.BulkInsertAsync(allManagers,
                                    new BulkConfig
                                    {
                                        IncludeGraph = true,
                                        UseUnlogged = true,
                                    },
                        cancellationToken: cancellationToken);
    }

    private async Task VanillaEfCoreAsync(List<Manager> allManagers, CancellationToken cancellationToken)
    {
        // This works, possibly better to wrap in a transaction, but not relevant to this example.
        foreach (var managers in allManagers.Chunk(10))
        {
            using var context = contextFactory.Create();
            context.Set<Manager>().AddRange(managers);
            await context.SaveChangesAsync(cancellationToken);
        }
    }
}