using FinancialService.Api.Extensions;
using FinancialService.Api.Middlewares;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration.AddEnvironmentVariables();

builder.Services
    .AddConfigurations(builder.Configuration)
    .AddHttpClient()
    .AddEndpointsApiExplorer()
    .AddSwaggerGen()
    .AddServices()
    .AddControllers();

WebApplication app = builder.Build();

app.UseMiddleware<RequestResponseLoggingMiddleware>();

app.UseMiddleware<ErrorHandlingMiddleware>();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.MapControllers();

app.UseCustomWebSocket();

await app.RunAsync();