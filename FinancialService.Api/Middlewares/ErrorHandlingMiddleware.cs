using System.Net;
using FinancialService.Api.Models;
using FinancialService.Application.Exceptions;

namespace FinancialService.Api.Middlewares;

public class ErrorHandlingMiddleware
{
    private const string _errorMessage = "Something went wrong";

    private readonly RequestDelegate _next;
    private readonly ILogger<ErrorHandlingMiddleware> _logger;

    public ErrorHandlingMiddleware(RequestDelegate next, ILogger<ErrorHandlingMiddleware> logger)
    {
        _logger = logger;
        _next = next;
    }

    public async Task InvokeAsync(HttpContext httpContext)
    {
        try
        {
            await _next(httpContext);
        }
        catch (ExceptionBase bex)
        {
            _logger.LogInformation(bex, bex.Message);

            httpContext.Response.StatusCode = bex.StatusCode;

            ErrorResponse error = new() { Message = bex.Message };

            await httpContext.Response.WriteAsJsonAsync(error);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, ex.Message);
            
            ErrorResponse errorResponse = new() { Message = _errorMessage };

            httpContext.Response.StatusCode = (int)HttpStatusCode.InternalServerError;

            await httpContext.Response.WriteAsJsonAsync(errorResponse);
        }
    }
}