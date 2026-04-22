using System;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Moq;
using Moq.Protected;
using Xunit;
using PROG7311_POE.Services;

namespace PROG7311_POE.Tests.Services
{
    public class RealTimeApiConverterTests
    {
        private static HttpClient CreateMockHttpClient(string jsonResponse, HttpStatusCode statusCode = HttpStatusCode.OK)
        {
            var handlerMock = new Mock<HttpMessageHandler>();
            handlerMock
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>()
                )
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = statusCode,
                    Content = new StringContent(jsonResponse, Encoding.UTF8, "application/json")
                });

            return new HttpClient(handlerMock.Object);
        }

        [Fact]
        public async Task ConvertUsdToZar_ValidResponse_ReturnsCorrectZarAmount()
        {
            // Arrange
            var mockJson = @"{ ""rates"": { ""ZAR"": 19.25 } }";
            var httpClient = CreateMockHttpClient(mockJson);
            var converter = new RealTimeApiConverter(httpClient);

            // Result
            var result = await converter.ConvertUsdToZar(100);

            // Assert
            Assert.Equal(1925m, result);
        }

        [Fact]
        public async Task ConvertUsdToZar_ApiReturnsDifferentRate_CalculatesCorrectly()
        {
            // Arrange
            var mockJson = @"{ ""rates"": { ""ZAR"": 18.75 } }";
            var httpClient = CreateMockHttpClient(mockJson);
            var converter = new RealTimeApiConverter(httpClient);

            // Result
            var result = await converter.ConvertUsdToZar(50);

            // Assert
            Assert.Equal(937.5m, result);
        }

        [Fact]
        public async Task ConvertUsdToZar_ZeroUsd_ReturnsZero()
        {
            var mockJson = @"{ ""rates"": { ""ZAR"": 19.25 } }";
            var httpClient = CreateMockHttpClient(mockJson);
            var converter = new RealTimeApiConverter(httpClient);

            var result = await converter.ConvertUsdToZar(0);

            Assert.Equal(0, result);
        }

        [Fact]
        public async Task ConvertUsdToZar_NegativeUsd_ReturnsNegativeZar()
        {
            var mockJson = @"{ ""rates"": { ""ZAR"": 19.25 } }";
            var httpClient = CreateMockHttpClient(mockJson);
            var converter = new RealTimeApiConverter(httpClient);

            var result = await converter.ConvertUsdToZar(-10);

            Assert.Equal(-192.5m, result);
        }

        [Fact]
        public async Task ConvertUsdToZar_ApiFailure_UsesFallbackRate()
        {
            var httpClient = CreateMockHttpClient("", HttpStatusCode.InternalServerError);
            var converter = new RealTimeApiConverter(httpClient);

            var result = await converter.ConvertUsdToZar(100);

            Assert.Equal(1850m, result);
        }

        [Fact]
        public async Task ConvertUsdToZar_InvalidJsonResponse_UsesFallbackRate()
        {
            var mockJson = @"{ ""invalid"": true }";
            var httpClient = CreateMockHttpClient(mockJson);
            var converter = new RealTimeApiConverter(httpClient);

            var result = await converter.ConvertUsdToZar(100);

            Assert.Equal(1850m, result);
        }

        [Fact]
        public async Task ConvertUsdToZar_MissingZarRate_UsesFallbackRate()
        {
            var mockJson = @"{ ""rates"": { ""USD"": 1 } }";
            var httpClient = CreateMockHttpClient(mockJson);
            var converter = new RealTimeApiConverter(httpClient);

            var result = await converter.ConvertUsdToZar(100);

            Assert.Equal(1850m, result);
        }
    }
}