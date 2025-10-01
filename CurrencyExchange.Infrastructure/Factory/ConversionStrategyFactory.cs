using System;
using System.Threading.Tasks;
using CurrencyExchange.Core.Interfaces;

namespace CurrencyExchange.Infrastructure.Factory
{
    public class ConversionStrategyFactory : IConversionStrategyFactory
    {
        private readonly IExchangeRateService _exchangeRateService;

        public ConversionStrategyFactory(IExchangeRateService exchangeRateService)
        {
            _exchangeRateService = exchangeRateService ?? throw new ArgumentNullException(nameof(exchangeRateService));
        }

        public IConversionStrategy CreateStrategy(string fromCurrency, string toCurrency)
        {
            // Same currency strategy
            if (string.Equals(fromCurrency, toCurrency, StringComparison.OrdinalIgnoreCase))
            {
                return new SameCurrencyStrategy();
            }

            // Direct conversion strategy (most cases)
            return new DirectConversionStrategy(_exchangeRateService);
        }
    }

    public class SameCurrencyStrategy : IConversionStrategy
    {
        public Task<decimal> ConvertAsync(decimal amount, string fromCurrency, string toCurrency)
        {
            return Task.FromResult(amount);
        }
    }

    public class DirectConversionStrategy : IConversionStrategy
    {
        private readonly IExchangeRateService _exchangeRateService;

        public DirectConversionStrategy(IExchangeRateService exchangeRateService)
        {
            _exchangeRateService = exchangeRateService ?? throw new ArgumentNullException(nameof(exchangeRateService));
        }

        public async Task<decimal> ConvertAsync(decimal amount, string fromCurrency, string toCurrency)
        {
            var exchangeRate = await _exchangeRateService.GetExchangeRateAsync(fromCurrency, toCurrency);
            return Math.Round(amount * exchangeRate, 2);
        }
    }
}