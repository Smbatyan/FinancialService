namespace FinancialService.Application.Exceptions;

public class BadRequestException : ExceptionBase
{
    public override int StatusCode { get; set; } = 400;
    public string Message { get; set; } = "bad_request";
    
    public BadRequestException(string message) : base(message)
    {
    }
}
