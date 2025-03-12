using EntityFrameworkCore.PostgreSQL.SimpleBulks.BulkInsert;
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
        });

        using var context = contextFactory.Create() as DatabaseContext;

        //var transaction = await context!.Database.BeginTransactionAsync(cancellationToken);

        context.BulkInsert(allManagers, options => { options.Timeout = 0; });


        foreach (var manager in allManagers)
        {
            Console.WriteLine($"Manager: {manager.Id} - {manager.DepartmentalId}");
        }

        //var newSessions = allSamples.Select(s => new Session
        //{
        //    SampleId = s.Id,
        //    ProjectSetupProjectId = projectSetupProjectId,
        //    QuotaId = autoFieldingQuotaId,
        //    OriginalQuotaId = autoFieldingQuotaId,
        //    QuotaVendorId = quotaVendorId,
        //    QuotaVendorLinkSettingsId = linkSettingsId,
        //    Status = SessionStatus.Eligible,
        //    IsTestSession = isTest
        //});

        //context.BulkInsert(newSessions, options => options.Timeout = 0);

        //await transaction.CommitAsync(cancellationToken);

        /*
         * Working EF Core code 
         
        context.Set<Manager>().AddRange(managers);
        await context.SaveChangesAsync(cancellationToken);

        foreach (var managers in allManagers.Chunk(10))
        {
            using var context = contextFactory.Create();
            context.Set<Manager>().AddRange(managers);
            await context.SaveChangesAsync(cancellationToken);
        }
        */
    }
}