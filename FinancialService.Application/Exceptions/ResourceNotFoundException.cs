namespace FinancialService.Application.Exceptions;

public class ResourceNotFoundException : ExceptionBase
{
    public ResourceNotFoundException(string message = "not_found") : base(message)
    {
    }

    public override int StatusCode { get; set; } = 404;
}