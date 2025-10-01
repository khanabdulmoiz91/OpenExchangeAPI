using System;

namespace CurrencyExchange.Core.Entities
{
    public class ConversionHistory
    {
        public int Id { get; set; }
        public string FromCurrency { get; set; } = string.Empty;
        public string ToCurrency { get; set; } = string.Empty;
        public decimal Amount { get; set; }
        public decimal ConvertedAmount { get; set; }
        public decimal ExchangeRate { get; set; }
        public DateTime ConversionDate { get; set; }
        public string? ClientIp { get; set; }
        
        public ConversionHistory()
        {
            ConversionDate = DateTime.UtcNow;
        }
    }
}