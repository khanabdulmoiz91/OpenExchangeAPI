# Additional Unit Tests to Implement

## Repository Tests
- **ConversionHistoryRepository Tests**
  - Test AddAsync with valid entity
  - Test AddAsync with null entity throws exception
  - Test GetByIdAsync returns correct entity
  - Test GetByIdAsync returns null for non-existent id
  - Test GetAllAsync returns ordered results
  - Test GetRecentAsync returns correct number of records
  - Test GetRecentAsync with zero count throws exception
  - Test SaveChangesAsync propagates to DbContext

## Controller Tests  
- **ExchangeController Tests**
  - Test ConvertCurrency with valid request returns 200 OK
  - Test ConvertCurrency with invalid model state returns 400
  - Test ConvertCurrency handles CurrencyNotSupportedException
  - Test ConvertCurrency handles ExchangeRateNotFoundException
  - Test ConvertCurrency handles ExternalApiException with 503
  - Test GetRates returns all rates for valid currency
  - Test GetRates handles invalid currency
  - Test GetHistory returns limited results
  - Test GetHistory respects maximum limit

## Factory Tests
- **ConversionStrategyFactory Tests**
  - Test CreateStrategy returns SameCurrencyStrategy for same currencies
  - Test CreateStrategy returns DirectConversionStrategy for different currencies
  - Test SameCurrencyStrategy returns same amount
  - Test DirectConversionStrategy calculates correctly

## Integration Tests
- **API Integration Tests**
  - Test full conversion flow end-to-end
  - Test database persistence after conversion
  - Test concurrent conversion requests
  - Test rate limiting behavior
  - Test API authentication/authorization if implemented

## Infrastructure Tests
- **OpenExchangeRates API Tests**
  - Test Refit client configuration
  - Test HTTP timeout handling
  - Test retry policy behavior
  - Test response deserialization

## Mapping Tests
- **AutoMapper Profile Tests**
  - Test ConversionHistory to ConvertCurrencyResponse mapping
  - Test ConversionHistory to ConversionHistoryDto mapping
  - Test all properties are mapped correctly

## Validation Tests
- **DTO Validation Tests**
  - Test ConvertCurrencyRequest validation attributes
  - Test currency code length validation
  - Test amount range validation
  
## Exception Handling Tests
- **Global Exception Handler Tests**
  - Test ProblemDetails formatting
  - Test different exception types produce correct status codes
  - Test error response structure

## Performance Tests
- **Service Performance Tests**
  - Test conversion service under load
  - Test caching behavior if implemented
  - Test database query optimization