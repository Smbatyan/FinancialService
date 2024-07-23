using FinancialService.Application.Exceptions;
using Newtonsoft.Json;

namespace FinancialService.Infrastructure.Models;

public class FxMessageResponse
{
    [JsonProperty("service")]
    public string Service { get; set; }

    [JsonProperty("messageType")]
    public string MessageType { get; set; }

    [JsonProperty("data")]
    public List<object>? Data { get; set; }

    [JsonIgnore]
    public decimal MidPrice
    {
        get
        {
            // Ensure there are at least 5 items and the 5th item is a double
            if (Data != null && Data.Count >= 5 && decimal.TryParse(Data[4]?.ToString(), out var result))
            {
                return result;
            }
            throw new BadRequestException("The data array does not contain a mid price.");
        }
    }
    
    [JsonIgnore]
    public string? Symbol
    {
        get
        {
            // Ensure there are at least 5 items and the 5th item is a double
            if (Data != null && Data.Count >= 5)
            {
                return Data[1]?.ToString();
            }
            throw new BadRequestException("The data array does not contain a valid symbol.");
        }
    }
}