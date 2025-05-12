using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using PS5_NOR_Modifier.Configuration;

namespace PS5_NOR_Modifier.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddInMemoryConfiguration(this IServiceCollection services)
    {
        var cfgBuilder = new ConfigurationBuilder();
        cfgBuilder.AddInMemoryCollection(InMemoryConfiguration.Configuration);
        var cfg = cfgBuilder.Build();
        services.AddSingleton<IConfiguration>(cfg);
        return services;
    }
}