using Newtonsoft.Json;

namespace FinancialService.Application.Models.Response;

public class TradeResponse
{
    [JsonProperty("s")]
    public string Symbol { get; set; }
    [JsonProperty("p")]
    public decimal Price { get; set; }
}
