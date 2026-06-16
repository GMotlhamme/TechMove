using Azure;
using System.Text.Json;
using TechMove.Interfaces;
using TechMove.Models.DTOs;

namespace TechMove.Services
{
    public class CurrencyApiAdapterService : ICurrencyService
    {
        private readonly HttpClient httpClient;

        public CurrencyApiAdapterService(HttpClient httpClient) {
            this.httpClient = httpClient;
        }

        public CurrencyApiAdapterService()
        {
        }

        public async Task<decimal> ConvertToZar(decimal amount)
        { 

                var response = await httpClient.GetAsync("https://v6.exchangerate-api.com/v6/108440d1b286453fdb046c09/latest/USD");


                if (!response.IsSuccessStatusCode)
                {
                    // Read the API error details (e.g., "invalid-key", "inactive-account")
                    string errorJson = await response.Content.ReadAsStringAsync();
                    throw new HttpRequestException($"API Error {response.StatusCode}: {errorJson}");
                }

                var json = await response.Content.ReadAsStringAsync();

                var data = JsonSerializer.Deserialize<ExchangeRateResponseDto>(json);

                string targetCurrency = "ZAR";
            
                bool foundRate = data.conversion_rates.TryGetValue(targetCurrency, out decimal rate);

                if (!foundRate)
                {
                    throw new Exception("ZAR exchange rate not found.");
                }

                decimal finalConversion = amount * rate;

                return finalConversion;
        }

        public decimal CalculateZar(decimal usd, decimal rate)
        {
            return usd * rate;
        }
    }
}
