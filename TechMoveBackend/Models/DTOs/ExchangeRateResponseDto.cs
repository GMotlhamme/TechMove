namespace TechMove.Models.DTOs
{
    public class ExchangeRateResponseDto
    {
        public string result { get; set; }
        public string base_code { get; set; }
        public Dictionary<string, decimal> conversion_rates { get; set; }
    }
}
