namespace TechMove.Interfaces
{
    public interface ICurrencyService
    {
        Task<decimal> ConvertToZar(decimal amount);
    }
}
