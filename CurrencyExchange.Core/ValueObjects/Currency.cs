using System;
using System.Collections.Generic;

namespace CurrencyExchange.Core.ValueObjects
{
    public class Currency
    {
        public string Code { get; }
        public string Name { get; }
        
        private static readonly Dictionary<string, string> SupportedCurrencies = new()
        {
            { "USD", "United States Dollar" },
            { "EUR", "Euro" },
            { "GBP", "British Pound Sterling" },
            { "JPY", "Japanese Yen" },
            { "AUD", "Australian Dollar" },
            { "CAD", "Canadian Dollar" },
            { "CHF", "Swiss Franc" },
            { "CNY", "Chinese Yuan" },
            { "SEK", "Swedish Krona" },
            { "NZD", "New Zealand Dollar" }
        };

        public Currency(string code)
        {
            if (string.IsNullOrWhiteSpace(code))
                throw new ArgumentException("Currency code cannot be empty", nameof(code));
            
            code = code.ToUpper();
            
            if (!SupportedCurrencies.ContainsKey(code))
                throw new ArgumentException($"Currency code '{code}' is not supported", nameof(code));

            Code = code;
            Name = SupportedCurrencies[code];
        }

        public static bool IsSupported(string code) => 
            !string.IsNullOrWhiteSpace(code) && SupportedCurrencies.ContainsKey(code.ToUpper());

        public static IEnumerable<Currency> GetAllCurrencies()
        {
            foreach (var currency in SupportedCurrencies)
            {
                yield return new Currency(currency.Key);
            }
        }

        public override string ToString() => $"{Code} - {Name}";
        
        public override bool Equals(object? obj)
        {
            if (obj is Currency other)
                return Code == other.Code;
            return false;
        }

        public override int GetHashCode() => Code.GetHashCode();
    }
}