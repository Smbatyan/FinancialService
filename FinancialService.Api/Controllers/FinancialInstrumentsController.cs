using System.Globalization;
using FinancialService.Api.Models;
using FinancialService.Application.Exceptions;
using FinancialService.Application.Interfaces;
using FinancialService.Core.Models;
using Microsoft.AspNetCore.Mvc;

namespace FinancialService.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class FinancialInstrumentsController : ControllerBase
    {
        private readonly ITiingoServiceClient _tiingoServiceClient;

        public FinancialInstrumentsController(ITiingoServiceClient tiingoServiceClient)
        {
            _tiingoServiceClient = tiingoServiceClient;
        }

        [HttpGet]
        public async Task<IActionResult> GetInstruments()
        {
            var instruments = await _tiingoServiceClient.GetAvailableInstruments();
            GetInstrumentsResponse response = new(instruments.Select(x => x.Symbol));
            return Ok(response);
        }

        [HttpGet("{symbol}")]
        public async Task<IActionResult> GetPrice(string symbol)
        {
            var price = await _tiingoServiceClient.GetCurrentPriceAsync(symbol);
            if (price is null)
            {
                throw new ResourceNotFoundException();
            }
            
            var response = new PriceResponse(price.Ticker, price.MidPrice);
            return Ok(response);
        }
    }
}