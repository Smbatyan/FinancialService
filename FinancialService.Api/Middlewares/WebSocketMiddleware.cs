using System.Net.WebSockets;
using System.Text;
using FinancialService.Infrastructure.Clients;
using Newtonsoft.Json;

namespace FinancialService.Api.Middlewares;

public class WebSocketMiddleware
{
    private readonly RequestDelegate _next;
    private readonly TiingoWebSocketClient _binanceWebSocketClient;

    public WebSocketMiddleware(RequestDelegate next, TiingoWebSocketClient binanceWebSocketClient)
    {
        _next = next;
        _binanceWebSocketClient = binanceWebSocketClient;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        if (context.Request.Path == "/ws" && context.WebSockets.IsWebSocketRequest)
        {
            using var webSocket = await context.WebSockets.AcceptWebSocketAsync();
            var clientId = Guid.NewGuid().ToString();

            await HandleWebSocketCommunication(webSocket, clientId);
        }
        else
        {
            await _next(context);
        }
    }

    private async Task HandleWebSocketCommunication(WebSocket webSocket, string clientId)
    {
        var buffer = new byte[1024 * 4];
        WebSocketReceiveResult result;

        do
        {
            result = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
            var message = Encoding.UTF8.GetString(buffer, 0, result.Count);

            var req = JsonConvert.DeserializeObject<UserReq>(message);
            if (req != null)
            {
                _binanceWebSocketClient.Subscribe(clientId, req.Ticker, async message =>
                {
                    var responseBuffer = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(message));
                    await webSocket.SendAsync(new ArraySegment<byte>(responseBuffer), WebSocketMessageType.Text, true,
                        CancellationToken.None);
                });
            }
        } while (!result.CloseStatus.HasValue);

        await webSocket.CloseAsync(result.CloseStatus.Value, result.CloseStatusDescription, CancellationToken.None);
    }
}

public class UserReq
{
    public string Ticker { get; set; }
}