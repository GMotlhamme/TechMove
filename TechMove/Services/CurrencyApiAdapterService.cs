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

                response.EnsureSuccessStatusCode();

                var json = await response.Content.ReadAsStringAsync();

                var data = JsonSerializer.Deserialize<ExchangeRateResponseDto>(json);

                var zarRate = data.rates.ZAR;

                decimal finalConversion = amount * zarRate;

                return finalConversion;   
        }

        public decimal CalculateZar(decimal usd, decimal rate)
        {
            return usd * rate;
        }
    }
}
