using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;

namespace TestBulkDbLibraries
{
    public static class ConfigurationHelper
    {
        public static TConfiguration GetConfiguration<TConfiguration>(string sectionName, IConfiguration configuration)
            where TConfiguration : class, new()
        {
            var section = configuration.GetSection(sectionName);

            var configurationInstance = new TConfiguration();

            new ConfigureFromConfigurationOptions<TConfiguration>(section).Configure(configurationInstance);

            return configurationInstance;
        }
    }
}
