using FinancialService.Application.Interfaces;
using FinancialService.Infrastructure.Clients;

namespace FinancialService.Api.Extensions;

public static class ServiceExtensions
{
    public static IServiceCollection AddServices(this IServiceCollection services)
    {
        services.AddSingleton<ITiingoServiceClient, TiingoServiceClient>();
        services.AddSingleton<TiingoWebSocketClient>();

        return services;
    }
}