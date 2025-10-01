using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using CurrencyExchange.Application.Exceptions;
using CurrencyExchange.Core.Interfaces;
using CurrencyExchange.Infrastructure.ExternalServices.Models;

namespace CurrencyExchange.Infrastructure.ExternalServices
{
    public class ExchangeRateService : IExchangeRateService
    {
        private readonly IOpenExchangeRatesApi _api;
        private readonly string _apiKey;

        public ExchangeRateService(IOpenExchangeRatesApi api, IConfiguration configuration)
        {
            _api = api ?? throw new ArgumentNullException(nameof(api));
            _apiKey = configuration["OpenExchangeRates:ApiKey"] ?? throw new InvalidOperationException("OpenExchangeRates:ApiKey not configured");
        }

        public async Task<decimal> GetExchangeRateAsync(string fromCurrency, string toCurrency)
        {
            try
            {
                fromCurrency = fromCurrency.ToUpper();
                toCurrency = toCurrency.ToUpper();

                // If converting same currency
                if (fromCurrency == toCurrency)
                    return 1;

                // Get rates with USD as base
                var response = await _api.GetLatestRatesAsync(_apiKey, "USD");
                
                if (response?.Rates == null)
                    throw new ExternalApiException("Invalid response from exchange rate API");

                // If from currency is USD
                if (fromCurrency == "USD")
                {
                    if (response.Rates.ContainsKey(toCurrency))
                        return response.Rates[toCurrency];
                    else
                        throw new ExchangeRateNotFoundException(fromCurrency, toCurrency);
                }

                // If to currency is USD
                if (toCurrency == "USD")
                {
                    if (response.Rates.ContainsKey(fromCurrency))
                        return 1 / response.Rates[fromCurrency];
                    else
                        throw new ExchangeRateNotFoundException(fromCurrency, toCurrency);
                }

                // Cross-rate calculation
                if (response.Rates.ContainsKey(fromCurrency) && response.Rates.ContainsKey(toCurrency))
                {
                    var fromRate = response.Rates[fromCurrency];
                    var toRate = response.Rates[toCurrency];
                    return toRate / fromRate;
                }

                throw new ExchangeRateNotFoundException(fromCurrency, toCurrency);
            }
            catch (Exception ex) when (!(ex is CurrencyExchangeException))
            {
                throw new ExternalApiException("Failed to fetch exchange rates", ex);
            }
        }

        public async Task<Dictionary<string, decimal>> GetAllRatesAsync(string baseCurrency)
        {
            try
            {
                baseCurrency = baseCurrency.ToUpper();
                
                // Get rates with USD as base first
                var response = await _api.GetLatestRatesAsync(_apiKey, "USD");
                
                if (response?.Rates == null)
                    throw new ExternalApiException("Invalid response from exchange rate API");

                // If base is USD, return as is
                if (baseCurrency == "USD")
                    return response.Rates;

                // Convert rates to requested base currency
                if (!response.Rates.ContainsKey(baseCurrency))
                    throw new CurrencyNotSupportedException(baseCurrency);

                var baseRate = response.Rates[baseCurrency];
                var convertedRates = new Dictionary<string, decimal>();

                foreach (var rate in response.Rates)
                {
                    if (rate.Key == baseCurrency)
                        convertedRates[rate.Key] = 1;
                    else
                        convertedRates[rate.Key] = rate.Value / baseRate;
                }

                // Add USD rate
                convertedRates["USD"] = 1 / baseRate;

                return convertedRates;
            }
            catch (Exception ex) when (!(ex is CurrencyExchangeException))
            {
                throw new ExternalApiException("Failed to fetch exchange rates", ex);
            }
        }
    }
}