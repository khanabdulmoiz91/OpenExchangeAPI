using System.Collections.Generic;
using System.Threading.Tasks;
using Refit;
using CurrencyExchange.Infrastructure.ExternalServices.Models;

namespace CurrencyExchange.Infrastructure.ExternalServices
{
    public interface IOpenExchangeRatesApi
    {
        [Get("/latest.json")]
        Task<OpenExchangeRatesResponse> GetLatestRatesAsync(
            [Query] string app_id,
            [Query] string @base = "USD",
            [Query] string? symbols = null);

        [Get("/currencies.json")]
        Task<Dictionary<string, string>> GetCurrenciesAsync();
    }
}