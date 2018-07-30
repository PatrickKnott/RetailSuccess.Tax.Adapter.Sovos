using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using RetailSuccess.Sovos.Client;

namespace RetailSuccess.Tax.Adapter.Sovos.SetUp
{
    public static class DependencyInjectionExtensions
    {
        public static IServiceCollection AddSovosTaxAdapter(this IServiceCollection services, IConfiguration config)
        {
            return services.Configure<SovosTaxClientOptions>(config.GetSection("SovosTaxClientOptions"))
                .AddSingleton(serviceProvider =>
                    serviceProvider.GetRequiredService<IOptions<SovosTaxClientOptions>>().Value)
                .AddTransient<ITaxHandler, SovosImplementation>();
        }
    }
}
