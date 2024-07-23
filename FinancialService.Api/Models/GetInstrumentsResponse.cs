namespace FinancialService.Api.Models;

public class GetInstrumentsResponse
{
    public GetInstrumentsResponse(IEnumerable<string> instruments)
    {
        Instruments = instruments;
    }
    
    public IEnumerable<string> Instruments { get; set; }
}