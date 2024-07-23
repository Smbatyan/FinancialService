using Newtonsoft.Json;

namespace FinancialService.Infrastructure.Models;

public class FxSubscriptionRequest
{
    [JsonProperty("eventName")]
    public string EventName { get; set; }

    [JsonProperty("authorization")]
    public string Authorization { get; set; }

    [JsonProperty("eventData")]
    public FxEventData EventData { get; set; }
}