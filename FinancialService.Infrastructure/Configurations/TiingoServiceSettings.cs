namespace FinancialService.Infrastructure.Configurations;

public class TiingoServiceSettings
{
    public List<string> ValidTickers { get; set; }
    public string ApiUrl { get; set; }
    public string Authorization { get; set; }
    public string SocketUri { get; set; }
}