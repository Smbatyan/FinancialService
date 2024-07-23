using FinancialService.Infrastructure.Configurations;

namespace FinancialService.Api.Extensions;

public static class ConfigurationExtension
{
    public static IServiceCollection AddConfigurations(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<TiingoServiceSettings>(configuration.GetSection("TiingoServiceSettings"));
        return services;
    }
}