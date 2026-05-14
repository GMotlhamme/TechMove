using TechMove.Interfaces;

namespace TechMove.Services
{
    public class CurrencyApiAdapterService : ICurrencyService
    {
        

        public async Task<decimal> ConvertToZar()
        {
            return 1;
        }
    }
}
