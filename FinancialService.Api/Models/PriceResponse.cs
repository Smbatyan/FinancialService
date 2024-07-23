namespace FinancialService.Api.Models;

public class PriceResponse
{
    public PriceResponse(string symbol, double price)
    {
        Symbol = symbol;
        Price = price;
    }
    public string Symbol { get; set; }
    public double Price { get; set; }
}