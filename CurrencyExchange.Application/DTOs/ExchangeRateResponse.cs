using System;
using System.Collections.Generic;

namespace CurrencyExchange.Application.DTOs
{
    public class ExchangeRateResponse
    {
        public string BaseCurrency { get; set; } = string.Empty;
        public Dictionary<string, decimal> Rates { get; set; } = new Dictionary<string, decimal>();
        public DateTime Timestamp { get; set; }
    }
}