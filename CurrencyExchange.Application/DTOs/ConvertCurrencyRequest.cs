using System.ComponentModel.DataAnnotations;

namespace CurrencyExchange.Application.DTOs
{
    public class ConvertCurrencyRequest
    {
        [Required(ErrorMessage = "From currency is required")]
        [StringLength(3, MinimumLength = 3, ErrorMessage = "Currency code must be exactly 3 characters")]
        public string FromCurrency { get; set; } = string.Empty;

        [Required(ErrorMessage = "To currency is required")]
        [StringLength(3, MinimumLength = 3, ErrorMessage = "Currency code must be exactly 3 characters")]
        public string ToCurrency { get; set; } = string.Empty;

        [Required(ErrorMessage = "Amount is required")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Amount must be greater than 0")]
        public decimal Amount { get; set; }
    }
}