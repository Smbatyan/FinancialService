using FinancialService.Api.Middlewares;
using FinancialService.Infrastructure.Clients;

namespace FinancialService.Api.Extensions;

public static class WebSocketExtensions
{
    public static IApplicationBuilder UseCustomWebSocket(this WebApplication builder)
    {
        builder.UseWebSockets();
        
        builder.Lifetime.ApplicationStarted.Register(() =>
        {
            var binanceWebSocketClient = builder.Services.GetService<TiingoWebSocketClient>();
            binanceWebSocketClient?.Connect();
        });

        builder.UseMiddleware<WebSocketMiddleware>();
        
        return builder;
    }
}