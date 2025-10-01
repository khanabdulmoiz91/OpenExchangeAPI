using System;
using System.Threading.Tasks;
using AutoFixture;
using AutoFixture.AutoMoq;
using AutoMapper;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using CurrencyExchange.Application.Exceptions;
using CurrencyExchange.Application.Services;
using CurrencyExchange.Core.Entities;
using CurrencyExchange.Core.Interfaces;

namespace CurrencyExchange.Tests.Services
{
    [TestClass]
    public class CurrencyConverterServiceTests
    {
        private IFixture _fixture = null!;
        private Mock<IExchangeRateService> _exchangeRateServiceMock = null!;
        private Mock<IConversionHistoryRepository> _repositoryMock = null!;
        private Mock<IConversionStrategyFactory> _strategyFactoryMock = null!;
        private Mock<IMapper> _mapperMock = null!;
        private CurrencyConverterService _service = null!;

        [TestInitialize]
        public void Setup()
        {
            _fixture = new Fixture().Customize(new AutoMoqCustomization());
            
            _exchangeRateServiceMock = _fixture.Freeze<Mock<IExchangeRateService>>();
            _repositoryMock = _fixture.Freeze<Mock<IConversionHistoryRepository>>();
            _strategyFactoryMock = _fixture.Freeze<Mock<IConversionStrategyFactory>>();
            _mapperMock = _fixture.Freeze<Mock<IMapper>>();
            
            _service = new CurrencyConverterService(
                _exchangeRateServiceMock.Object,
                _repositoryMock.Object,
                _strategyFactoryMock.Object,
                _mapperMock.Object);
        }

        [TestMethod]
        public async Task ConvertCurrencyAsync_ValidInput_ReturnsConversionHistory()
        {
            // Arrange
            var fromCurrency = "USD";
            var toCurrency = "EUR";
            var amount = 100m;
            var exchangeRate = 0.85m;
            var convertedAmount = amount * exchangeRate;
            
            var strategyMock = new Mock<IConversionStrategy>();
            strategyMock.Setup(s => s.ConvertAsync(amount, fromCurrency, toCurrency))
                .ReturnsAsync(convertedAmount);
            
            _strategyFactoryMock.Setup(f => f.CreateStrategy(fromCurrency, toCurrency))
                .Returns(strategyMock.Object);
            
            _exchangeRateServiceMock.Setup(s => s.GetExchangeRateAsync(fromCurrency, toCurrency))
                .ReturnsAsync(exchangeRate);
            
            _repositoryMock.Setup(r => r.AddAsync(It.IsAny<ConversionHistory>()))
                .ReturnsAsync((ConversionHistory h) => h);
            
            _repositoryMock.Setup(r => r.SaveChangesAsync())
                .ReturnsAsync(1);

            // Act
            var result = await _service.ConvertCurrencyAsync(fromCurrency, toCurrency, amount);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(fromCurrency, result.FromCurrency);
            Assert.AreEqual(toCurrency, result.ToCurrency);
            Assert.AreEqual(amount, result.Amount);
            Assert.AreEqual(convertedAmount, result.ConvertedAmount);
            Assert.AreEqual(exchangeRate, result.ExchangeRate);
            
            _repositoryMock.Verify(r => r.AddAsync(It.IsAny<ConversionHistory>()), Times.Once);
            _repositoryMock.Verify(r => r.SaveChangesAsync(), Times.Once);
        }

        [TestMethod]
        [ExpectedException(typeof(CurrencyNotSupportedException))]
        public async Task ConvertCurrencyAsync_InvalidFromCurrency_ThrowsException()
        {
            // Arrange
            var fromCurrency = "XXX";
            var toCurrency = "EUR";
            var amount = 100m;

            // Act
            await _service.ConvertCurrencyAsync(fromCurrency, toCurrency, amount);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public async Task ConvertCurrencyAsync_NegativeAmount_ThrowsException()
        {
            // Arrange
            var fromCurrency = "USD";
            var toCurrency = "EUR";
            var amount = -100m;

            // Act
            await _service.ConvertCurrencyAsync(fromCurrency, toCurrency, amount);
        }

        [TestMethod]
        public async Task ConvertCurrencyAsync_WithClientIp_StoresIpInHistory()
        {
            // Arrange
            var fromCurrency = "USD";
            var toCurrency = "EUR";
            var amount = 100m;
            var clientIp = "192.168.1.1";
            var exchangeRate = 0.85m;
            
            var strategyMock = new Mock<IConversionStrategy>();
            strategyMock.Setup(s => s.ConvertAsync(amount, fromCurrency, toCurrency))
                .ReturnsAsync(amount * exchangeRate);
            
            _strategyFactoryMock.Setup(f => f.CreateStrategy(fromCurrency, toCurrency))
                .Returns(strategyMock.Object);
            
            _exchangeRateServiceMock.Setup(s => s.GetExchangeRateAsync(fromCurrency, toCurrency))
                .ReturnsAsync(exchangeRate);
            
            ConversionHistory? capturedHistory = null;
            _repositoryMock.Setup(r => r.AddAsync(It.IsAny<ConversionHistory>()))
                .Callback<ConversionHistory>(h => capturedHistory = h)
                .ReturnsAsync((ConversionHistory h) => h);

            // Act
            await _service.ConvertCurrencyAsync(fromCurrency, toCurrency, amount, clientIp);

            // Assert
            Assert.IsNotNull(capturedHistory);
            Assert.AreEqual(clientIp, capturedHistory.ClientIp);
        }
    }
}