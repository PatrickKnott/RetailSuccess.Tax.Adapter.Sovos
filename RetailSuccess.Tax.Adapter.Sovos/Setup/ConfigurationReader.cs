using Microsoft.Extensions.Configuration;
using RetailSuccess.Sovos.Client;

namespace RetailSuccess.Tax.Adapter.Sovos.SetUp
{
    internal static class ConfigurationReader
    {
        internal static SovosTaxClientOptions GetAppSettings()
        {
            var configBuilder = new ConfigurationBuilder();
            configBuilder.AddJsonFile("appSettings.json")
                .AddJsonFile("appSettings.Development.json");
            var config = configBuilder.Build();
            return config.Get<SovosTaxClientOptions>();
        }
    }
}
