using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace CurrencyExchange.Infrastructure.ExternalServices.Models
{
    public class OpenExchangeRatesResponse
    {
        [JsonProperty("disclaimer")]
        public string Disclaimer { get; set; } = string.Empty;

        [JsonProperty("license")]
        public string License { get; set; } = string.Empty;

        [JsonProperty("timestamp")]
        public long Timestamp { get; set; }

        [JsonProperty("base")]
        public string Base { get; set; } = string.Empty;

        [JsonProperty("rates")]
        public Dictionary<string, decimal> Rates { get; set; } = new Dictionary<string, decimal>();
    }
}