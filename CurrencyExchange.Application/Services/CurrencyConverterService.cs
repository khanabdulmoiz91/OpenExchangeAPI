using System;
using System.Threading.Tasks;
using AutoMapper;
using CurrencyExchange.Application.Exceptions;
using CurrencyExchange.Core.Entities;
using CurrencyExchange.Core.Interfaces;
using CurrencyExchange.Core.ValueObjects;

namespace CurrencyExchange.Application.Services
{
    public class CurrencyConverterService : ICurrencyConverterService
    {
        private readonly IExchangeRateService _exchangeRateService;
        private readonly IConversionHistoryRepository _repository;
        private readonly IConversionStrategyFactory _strategyFactory;
        private readonly IMapper _mapper;

        public CurrencyConverterService(
            IExchangeRateService exchangeRateService,
            IConversionHistoryRepository repository,
            IConversionStrategyFactory strategyFactory,
            IMapper mapper)
        {
            _exchangeRateService = exchangeRateService ?? throw new ArgumentNullException(nameof(exchangeRateService));
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
            _strategyFactory = strategyFactory ?? throw new ArgumentNullException(nameof(strategyFactory));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        public async Task<ConversionHistory> ConvertCurrencyAsync(string fromCurrency, string toCurrency, decimal amount, string? clientIp = null)
        {
            // Validate currencies
            if (!Currency.IsSupported(fromCurrency))
                throw new CurrencyNotSupportedException(fromCurrency);
                
            if (!Currency.IsSupported(toCurrency))
                throw new CurrencyNotSupportedException(toCurrency);

            if (amount <= 0)
                throw new ArgumentException("Amount must be greater than zero", nameof(amount));

            // Use factory pattern to get conversion strategy
            var strategy = _strategyFactory.CreateStrategy(fromCurrency, toCurrency);
            var convertedAmount = await strategy.ConvertAsync(amount, fromCurrency, toCurrency);
            
            // Get exchange rate for logging
            var exchangeRate = await _exchangeRateService.GetExchangeRateAsync(fromCurrency, toCurrency);

            // Create and save conversion history
            var history = new ConversionHistory
            {
                FromCurrency = fromCurrency.ToUpper(),
                ToCurrency = toCurrency.ToUpper(),
                Amount = amount,
                ConvertedAmount = convertedAmount,
                ExchangeRate = exchangeRate,
                ClientIp = clientIp,
                ConversionDate = DateTime.UtcNow
            };

            await _repository.AddAsync(history);
            await _repository.SaveChangesAsync();

            return history;
        }
    }
}