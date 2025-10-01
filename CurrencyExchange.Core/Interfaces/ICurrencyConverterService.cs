using System.Threading.Tasks;
using CurrencyExchange.Core.Entities;

namespace CurrencyExchange.Core.Interfaces
{
    public interface ICurrencyConverterService
    {
        Task<ConversionHistory> ConvertCurrencyAsync(string fromCurrency, string toCurrency, decimal amount, string? clientIp = null);
    }
}