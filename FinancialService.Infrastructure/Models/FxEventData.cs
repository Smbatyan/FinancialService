using Newtonsoft.Json;

namespace FinancialService.Infrastructure.Models;

public class FxEventData
{
    [JsonProperty("thresholdLevel")]
    public int ThresholdLevel { get; set; }

    [JsonProperty("tickers")]
    public List<string> Tickers { get; set; }
}