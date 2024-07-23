using System.Collections.Concurrent;
using System.Net.WebSockets;
using System.Text;
using FinancialService.Application.Interfaces;
using FinancialService.Application.Models.Response;
using FinancialService.Infrastructure.Configurations;
using FinancialService.Infrastructure.Models;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace FinancialService.Infrastructure.Clients;

public class TiingoWebSocketClient : ITiingoWebSocketClient
{
    private readonly ILogger<TiingoWebSocketClient> _logger;

    private readonly TiingoServiceSettings _tiingoServiceSettings;
    private readonly ClientWebSocket _webSocket;
    private readonly ConcurrentDictionary<string, Action<SubscriptionResponse>> _subscribers;
    private readonly HashSet<string> _subscribedSymbols;
    private readonly object _lock = new();
    private CancellationTokenSource _cancellationTokenSource;


    public TiingoWebSocketClient(ILogger<TiingoWebSocketClient> logger,
        IOptions<TiingoServiceSettings> financialServiceSettings)
    {
        _logger = logger;
        _tiingoServiceSettings = financialServiceSettings.Value;
        _webSocket = new ClientWebSocket();
        _webSocket.Options.RemoteCertificateValidationCallback = (_, _, _, _) => true;
        _subscribers = new ConcurrentDictionary<string, Action<SubscriptionResponse>>();
        _subscribedSymbols = new HashSet<string>();
        _cancellationTokenSource = new CancellationTokenSource();

        Connect().GetAwaiter().GetResult();
    }

    public async Task Connect()
    {
        try
        {
            if (_webSocket.State != WebSocketState.Open)
            {
                _logger.LogInformation("Attempting to connect to WebSocket...");

                await _webSocket.ConnectAsync(new Uri(_tiingoServiceSettings.SocketUri),
                    _cancellationTokenSource.Token);

                _logger.LogInformation("WebSocket connection established.");

                StartReceiveLoop();
            }
        }
        catch (Exception ex)
        {
            _logger.LogError("An error occurred while connecting: " + ex.Message, ex);
        }
    }

    public void Subscribe(string clientId, string symbol, Action<SubscriptionResponse> callback)
    {
        lock (_lock)
        {
            if (!_subscribedSymbols.Contains(symbol))
            {
                SubscribeToPriceUpdates(symbol);
                _subscribedSymbols.Add(symbol);
            }
        }

        _subscribers[$"{clientId}-{symbol}"] = callback;
        _logger.LogInformation($"Client {clientId} subscribed to {symbol}.");
    }

    private async void SubscribeToPriceUpdates(string symbol)
    {
        var subMessage = new FxSubscriptionRequest
        {
            Authorization = _tiingoServiceSettings.Authorization,
            EventName = "subscribe",
            EventData = new()
            {
                ThresholdLevel = 5,
                Tickers = [symbol]
            },
        };

        var message = JsonConvert.SerializeObject(subMessage);
        await SendMessageAsync(message);
        _logger.LogInformation($"Subscribed to {symbol} updates.");
    }

    private async Task SendMessageAsync(string message)
    {
        if (_webSocket.State == WebSocketState.Open)
        {
            var bytes = Encoding.UTF8.GetBytes(message);
            var buffer = new ArraySegment<byte>(bytes);
            await _webSocket.SendAsync(buffer, WebSocketMessageType.Text, true, _cancellationTokenSource.Token);
        }
        else
        {
            _logger.LogWarning("WebSocket connection is closed. Attempting to reconnect...");
            await Connect();
            if (_webSocket.State == WebSocketState.Open)
            {
                var bytes = Encoding.UTF8.GetBytes(message);
                var buffer = new ArraySegment<byte>(bytes);
                await _webSocket.SendAsync(buffer, WebSocketMessageType.Text, true, _cancellationTokenSource.Token);
            }
            else
            {
                _logger.LogError("Failed to send message: WebSocket connection is not alive.");
            }
        }
    }

    private async void StartReceiveLoop()
    {
        var buffer = new byte[1024 * 4];
        while (_webSocket.State == WebSocketState.Open)
        {
            try
            {
                WebSocketReceiveResult result = await _webSocket.ReceiveAsync(new ArraySegment<byte>(buffer),
                    _cancellationTokenSource.Token);
                if (result.MessageType == WebSocketMessageType.Close)
                {
                    await _webSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, string.Empty,
                        _cancellationTokenSource.Token);
                    _logger.LogInformation("WebSocket connection closed.");
                }
                else
                {
                    var message = Encoding.UTF8.GetString(buffer, 0, result.Count);
                    OnMessageReceived(message);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError("An error occurred while receiving a message: " + ex.Message, ex);
            }
        }
    }

    private void OnMessageReceived(string message)
    {
        FxMessageResponse? tradeResponse = default;
        _logger.LogInformation($"Received message: {message}");

        try
        {
            tradeResponse = JsonConvert.DeserializeObject<FxMessageResponse>(message);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error deserializing message");
            return;
        }
        
        if (tradeResponse != null && !string.IsNullOrEmpty(tradeResponse.Symbol))
        {
            foreach (var subscriber in _subscribers)
            {
                if (subscriber.Key.EndsWith(tradeResponse.Symbol, StringComparison.OrdinalIgnoreCase))
                {
                    subscriber.Value(new SubscriptionResponse
                        { Price = tradeResponse.MidPrice, Symbol = tradeResponse.Symbol });
                }
            }
        }
    }
}