using FinancialService.Application.Models.Response;

namespace FinancialService.Application.Interfaces;

public interface ITiingoWebSocketClient
{
    Task Connect();
    void Subscribe(string clientId, string symbol, Action<SubscriptionResponse> callback);
}