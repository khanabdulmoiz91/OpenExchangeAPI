using System.Threading.Tasks;

namespace CurrencyExchange.Core.Interfaces
{
    public interface IConversionStrategyFactory
    {
        IConversionStrategy CreateStrategy(string fromCurrency, string toCurrency);
    }

    public interface IConversionStrategy
    {
        Task<decimal> ConvertAsync(decimal amount, string fromCurrency, string toCurrency);
    }
}