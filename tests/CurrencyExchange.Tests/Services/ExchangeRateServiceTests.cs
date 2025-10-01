using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoFixture;
using AutoFixture.AutoMoq;
using Microsoft.Extensions.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using CurrencyExchange.Application.Exceptions;
using CurrencyExchange.Infrastructure.ExternalServices;
using CurrencyExchange.Infrastructure.ExternalServices.Models;

namespace CurrencyExchange.Tests.Services
{
    [TestClass]
    public class ExchangeRateServiceTests
    {
        private IFixture _fixture = null!;
        private Mock<IOpenExchangeRatesApi> _apiMock = null!;
        private Mock<IConfiguration> _configurationMock = null!;
        private ExchangeRateService _service = null!;

        [TestInitialize]
        public void Setup()
        {
            _fixture = new Fixture().Customize(new AutoMoqCustomization());
            
            _apiMock = _fixture.Freeze<Mock<IOpenExchangeRatesApi>>();
            _configurationMock = _fixture.Freeze<Mock<IConfiguration>>();
            
            _configurationMock.Setup(c => c["OpenExchangeRates:ApiKey"])
                .Returns("test-api-key");
            
            _service = new ExchangeRateService(_apiMock.Object, _configurationMock.Object);
        }

        [TestMethod]
        public async Task GetExchangeRateAsync_SameCurrency_ReturnsOne()
        {
            // Arrange
            var currency = "USD";

            // Act
            var result = await _service.GetExchangeRateAsync(currency, currency);

            // Assert
            Assert.AreEqual(1m, result);
            _apiMock.Verify(a => a.GetLatestRatesAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string?>()), Times.Never);
        }

        [TestMethod]
        public async Task GetExchangeRateAsync_FromUSD_ReturnsDirectRate()
        {
            // Arrange
            var fromCurrency = "USD";
            var toCurrency = "EUR";
            var expectedRate = 0.85m;
            
            var response = new OpenExchangeRatesResponse
            {
                Base = "USD",
                Rates = new Dictionary<string, decimal>
                {
                    { "EUR", expectedRate }
                }
            };
            
            _apiMock.Setup(a => a.GetLatestRatesAsync(It.IsAny<string>(), "USD", It.IsAny<string?>()))
                .ReturnsAsync(response);

            // Act
            var result = await _service.GetExchangeRateAsync(fromCurrency, toCurrency);

            // Assert
            Assert.AreEqual(expectedRate, result);
        }

        [TestMethod]
        public async Task GetExchangeRateAsync_ToUSD_ReturnsInverseRate()
        {
            // Arrange
            var fromCurrency = "EUR";
            var toCurrency = "USD";
            var eurRate = 0.85m;
            var expectedRate = 1 / eurRate;
            
            var response = new OpenExchangeRatesResponse
            {
                Base = "USD",
                Rates = new Dictionary<string, decimal>
                {
                    { "EUR", eurRate }
                }
            };
            
            _apiMock.Setup(a => a.GetLatestRatesAsync(It.IsAny<string>(), "USD", It.IsAny<string?>()))
                .ReturnsAsync(response);

            // Act
            var result = await _service.GetExchangeRateAsync(fromCurrency, toCurrency);

            // Assert
            Assert.AreEqual(expectedRate, result, 0.0001m);
        }

        [TestMethod]
        public async Task GetExchangeRateAsync_CrossRate_CalculatesCorrectly()
        {
            // Arrange
            var fromCurrency = "EUR";
            var toCurrency = "GBP";
            var eurRate = 0.85m;
            var gbpRate = 0.73m;
            var expectedRate = gbpRate / eurRate;
            
            var response = new OpenExchangeRatesResponse
            {
                Base = "USD",
                Rates = new Dictionary<string, decimal>
                {
                    { "EUR", eurRate },
                    { "GBP", gbpRate }
                }
            };
            
            _apiMock.Setup(a => a.GetLatestRatesAsync(It.IsAny<string>(), "USD", It.IsAny<string?>()))
                .ReturnsAsync(response);

            // Act
            var result = await _service.GetExchangeRateAsync(fromCurrency, toCurrency);

            // Assert
            Assert.AreEqual(expectedRate, result, 0.0001m);
        }

        [TestMethod]
        [ExpectedException(typeof(ExternalApiException))]
        public async Task GetExchangeRateAsync_NullResponse_ThrowsException()
        {
            // Arrange
            _apiMock.Setup(a => a.GetLatestRatesAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string?>()))
                .ReturnsAsync((OpenExchangeRatesResponse?)null);

            // Act
            await _service.GetExchangeRateAsync("USD", "EUR");
        }

        [TestMethod]
        [ExpectedException(typeof(ExchangeRateNotFoundException))]
        public async Task GetExchangeRateAsync_CurrencyNotInResponse_ThrowsException()
        {
            // Arrange
            var response = new OpenExchangeRatesResponse
            {
                Base = "USD",
                Rates = new Dictionary<string, decimal>
                {
                    { "EUR", 0.85m }
                }
            };
            
            _apiMock.Setup(a => a.GetLatestRatesAsync(It.IsAny<string>(), "USD", It.IsAny<string?>()))
                .ReturnsAsync(response);

            // Act
            await _service.GetExchangeRateAsync("USD", "XXX");
        }
    }
}