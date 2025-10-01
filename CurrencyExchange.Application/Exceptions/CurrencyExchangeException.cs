using System;

namespace CurrencyExchange.Application.Exceptions
{
    public class CurrencyExchangeException : Exception
    {
        public string? ErrorCode { get; }

        public CurrencyExchangeException(string message, string? errorCode = null) 
            : base(message)
        {
            ErrorCode = errorCode;
        }

        public CurrencyExchangeException(string message, Exception? innerException, string? errorCode = null) 
            : base(message, innerException)
        {
            ErrorCode = errorCode;
        }
    }

    public class CurrencyNotSupportedException : CurrencyExchangeException
    {
        public CurrencyNotSupportedException(string currency) 
            : base($"Currency '{currency}' is not supported", "CURRENCY_NOT_SUPPORTED")
        {
        }
    }

    public class ExchangeRateNotFoundException : CurrencyExchangeException
    {
        public ExchangeRateNotFoundException(string fromCurrency, string toCurrency) 
            : base($"Exchange rate not found for {fromCurrency} to {toCurrency}", "RATE_NOT_FOUND")
        {
        }
    }

    public class ExternalApiException : CurrencyExchangeException
    {
        public ExternalApiException(string message, Exception? innerException = null) 
            : base(message, innerException, "EXTERNAL_API_ERROR")
        {
        }
    }
}