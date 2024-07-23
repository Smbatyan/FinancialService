namespace FinancialService.Application.Exceptions;

public abstract class ExceptionBase : ApplicationException
{
    public abstract int StatusCode { get; set; }
    
    protected ExceptionBase(string message) : base(message)
    {
    }
}