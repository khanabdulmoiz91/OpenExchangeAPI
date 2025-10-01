using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using CurrencyExchange.Application.DTOs;
using CurrencyExchange.Application.Exceptions;
using CurrencyExchange.Core.Interfaces;

namespace CurrencyExchange.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]
    public class ExchangeController : ControllerBase
    {
        private readonly ICurrencyConverterService _converterService;
        private readonly IConversionHistoryRepository _historyRepository;
        private readonly IExchangeRateService _rateService;
        private readonly IMapper _mapper;

        public ExchangeController(
            ICurrencyConverterService converterService,
            IConversionHistoryRepository historyRepository,
            IExchangeRateService rateService,
            IMapper mapper)
        {
            _converterService = converterService ?? throw new ArgumentNullException(nameof(converterService));
            _historyRepository = historyRepository ?? throw new ArgumentNullException(nameof(historyRepository));
            _rateService = rateService ?? throw new ArgumentNullException(nameof(rateService));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        /// <summary>
        /// Convert currency from one type to another
        /// </summary>
        /// <param name="request">Conversion request details</param>
        /// <returns>Conversion result with exchange rate details</returns>
        /// <response code="200">Returns the conversion result</response>
        /// <response code="400">If the request is invalid or currency is not supported</response>
        /// <response code="503">If the external exchange rate service is unavailable</response>
        [HttpPost("convert")]
        [ProducesResponseType(typeof(ConvertCurrencyResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status503ServiceUnavailable)]
        public async Task<ActionResult<ConvertCurrencyResponse>> ConvertCurrency([FromBody] ConvertCurrencyRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var clientIp = HttpContext.Connection.RemoteIpAddress?.ToString();
                var result = await _converterService.ConvertCurrencyAsync(
                    request.FromCurrency,
                    request.ToCurrency,
                    request.Amount,
                    clientIp);

                var response = _mapper.Map<ConvertCurrencyResponse>(result);
                return Ok(response);
            }
            catch (CurrencyNotSupportedException ex)
            {
                return Problem(
                    detail: ex.Message,
                    statusCode: StatusCodes.Status400BadRequest,
                    title: "Invalid Currency",
                    type: "https://example.com/errors/invalid-currency");
            }
            catch (ExchangeRateNotFoundException ex)
            {
                return Problem(
                    detail: ex.Message,
                    statusCode: StatusCodes.Status400BadRequest,
                    title: "Exchange Rate Not Found",
                    type: "https://example.com/errors/rate-not-found");
            }
            catch (ExternalApiException ex)
            {
                return Problem(
                    detail: ex.Message,
                    statusCode: StatusCodes.Status503ServiceUnavailable,
                    title: "External Service Error",
                    type: "https://example.com/errors/external-service");
            }
        }

        /// <summary>
        /// Get exchange rates for a specific currency
        /// </summary>
        /// <param name="currency">Base currency code (e.g., USD, EUR)</param>
        /// <returns>Exchange rates for all supported currencies</returns>
        /// <response code="200">Returns the exchange rates</response>
        /// <response code="400">If the currency is not supported</response>
        [HttpGet("rates/{currency}")]
        [ProducesResponseType(typeof(ExchangeRateResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<ExchangeRateResponse>> GetRates(string currency)
        {
            try
            {
                var rates = await _rateService.GetAllRatesAsync(currency);
                var response = new ExchangeRateResponse
                {
                    BaseCurrency = currency.ToUpper(),
                    Rates = rates,
                    Timestamp = DateTime.UtcNow
                };
                return Ok(response);
            }
            catch (CurrencyNotSupportedException ex)
            {
                return Problem(
                    detail: ex.Message,
                    statusCode: StatusCodes.Status400BadRequest,
                    title: "Invalid Currency",
                    type: "https://example.com/errors/invalid-currency");
            }
            catch (ExternalApiException ex)
            {
                return Problem(
                    detail: ex.Message,
                    statusCode: StatusCodes.Status503ServiceUnavailable,
                    title: "External Service Error",
                    type: "https://example.com/errors/external-service");
            }
        }

        /// <summary>
        /// Get conversion history
        /// </summary>
        /// <param name="limit">Maximum number of records to return (default: 10)</param>
        /// <returns>List of recent conversions</returns>
        /// <response code="200">Returns the conversion history</response>
        [HttpGet("history")]
        [ProducesResponseType(typeof(IEnumerable<ConversionHistoryDto>), StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<ConversionHistoryDto>>> GetHistory([FromQuery] int limit = 10)
        {
            var history = await _historyRepository.GetRecentAsync(Math.Min(limit, 100));
            var response = _mapper.Map<IEnumerable<ConversionHistoryDto>>(history);
            return Ok(response);
        }
    }
}