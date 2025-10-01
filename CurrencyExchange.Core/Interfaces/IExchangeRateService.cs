using System.Collections.Generic;
using System.Threading.Tasks;

namespace CurrencyExchange.Core.Interfaces
{
    public interface IExchangeRateService
    {
        Task<decimal> GetExchangeRateAsync(string fromCurrency, string toCurrency);
        Task<Dictionary<string, decimal>> GetAllRatesAsync(string baseCurrency);
    }
}