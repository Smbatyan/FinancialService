using FinancialService.Application.Models.Response;
using FinancialService.Core.Models;

namespace FinancialService.Application.Interfaces;

public interface ITiingoServiceClient
{
    Task<IEnumerable<FinancialInstrument>> GetAvailableInstruments();
    Task<ForexQuoteResponse?> GetCurrentPriceAsync(string symbol);
}