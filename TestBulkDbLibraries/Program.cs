using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Npgsql;
using TestBulkDbLibraries;
using TestBulkDbLibraries.Database;

HostApplicationBuilder builder = Host.CreateApplicationBuilder(args);

builder.Services.AddTransient<ITester, Tester>();
builder.Services.AddEntityFrameworkNpgsql();

IConfiguration configuration = new ConfigurationBuilder()
    .AddJsonFile("appSettings.json", optional: false)
    .Build();

RegisterDatabaseContext(builder.Services, configuration);


using IHost host = builder
    .Build();

RunTester(host.Services);

await host.RunAsync();

// Ensure console window doesn't close.
Console.ReadLine();

static void RunTester(IServiceProvider hostProvider)
{
    using IServiceScope scope = hostProvider.CreateScope();
    IServiceProvider provider = scope.ServiceProvider;
    var tester = provider.GetRequiredService<ITester>();
    var idsToImport = Enumerable.Range(0, 10).Select(i => Guid.NewGuid().ToString()).ToList();
    tester.RunBulkImport(idsToImport, 5, 7, 6, 4, 20, 15, true, CancellationToken.None).Wait();
}


static void RegisterDatabaseContext(IServiceCollection services, IConfiguration configuration)
{
    var entityFrameworkConfiguration = ConfigurationHelper.GetConfiguration<EntityFrameworkConfiguration>("MasterDatabase", configuration);
    services.AddSingleton(entityFrameworkConfiguration);

    var postgreSqlOptionsBuilder = new DbContextOptionsBuilder<DatabaseContext>();

    var dataSourceBuilder = new NpgsqlDataSourceBuilder(entityFrameworkConfiguration.DatabaseConnectionString);

    var dataSource = dataSourceBuilder.Build();

    postgreSqlOptionsBuilder.UseNpgsql(dataSource);
    postgreSqlOptionsBuilder.UseSnakeCaseNamingConvention();

    // Register services.
    services.AddSingleton<IDatabaseContextFactory>(s => new DatabaseContextFactory(postgreSqlOptionsBuilder.Options));

    services.AddTransient(provider =>
    {
        IDatabaseContextFactory factory = provider.GetService<IDatabaseContextFactory>() ?? throw new InvalidOperationException();
        return factory.Create();
    });
}