using TechMove.Models.Domain;

namespace TechMove.Interfaces
{
    public interface IContractObserver
    {
        Task UpdateContractStatusAsync(Contract contract);
    }
}
