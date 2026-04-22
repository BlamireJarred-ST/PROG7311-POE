using System;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

namespace PROG7311_POE.Services
{
    // currency converter API
    public class RealTimeApiConverter : ICurrencyConverter
    {
        private readonly HttpClient _httpClient; 
        private const string ApiUrl = "https://open.er-api.com/v6/latest/USD";

        public RealTimeApiConverter(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<decimal> ConvertUsdToZar(decimal usdAmount)
        {
            try
            {
                // Call the exchange rate API
                var response = await _httpClient.GetAsync(ApiUrl);
                response.EnsureSuccessStatusCode();

                var json = await response.Content.ReadAsStringAsync();

                // JSON response
                using var doc = JsonDocument.Parse(json);
                var zarRate = doc.RootElement
                    .GetProperty("rates")
                    .GetProperty("ZAR")
                    .GetDecimal();

                // Perform conversion
                return usdAmount * zarRate;
            }
            catch (Exception)
            {
                // Fallback rate if API is down
                return usdAmount * 18.50m;
            }
        }
    }
}