using System.Net.Http.Json;
using FinancialService.Application.Exceptions;
using FinancialService.Application.Interfaces;
using FinancialService.Application.Models.Response;
using FinancialService.Core.Models;
using FinancialService.Infrastructure.Configurations;
using Microsoft.Extensions.Options;

namespace FinancialService.Infrastructure.Clients;

public class TiingoServiceClient : ITiingoServiceClient
{
    private readonly HttpClient _httpClient;
    private readonly TiingoServiceSettings _settings;

    public TiingoServiceClient(HttpClient httpClient, IOptions<TiingoServiceSettings> settings)
    {
        _httpClient = httpClient;
        _settings = settings.Value;
    }

    public Task<IEnumerable<FinancialInstrument>> GetAvailableInstruments()
    {
        return Task.FromResult(_settings.ValidTickers
            .Select(symbol => new FinancialInstrument { Symbol = symbol }));
    }

    public async Task<ForexQuoteResponse?> GetCurrentPriceAsync(string symbol)
    {
        if (!_settings.ValidTickers.Contains(symbol))
        {
            throw new BadRequestException("the requested symbol is not supported");
        }

        string uri = string.Format(_settings.ApiUrl, symbol, _settings.Authorization);

        var response = await _httpClient.GetFromJsonAsync<List<ForexQuoteResponse>>(uri);

        return response.FirstOrDefault();
    }
}