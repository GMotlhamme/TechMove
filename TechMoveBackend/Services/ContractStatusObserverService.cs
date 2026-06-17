using TechMove.Data;
using TechMove.Interfaces;
using TechMove.Models.Domain;

namespace TechMove.Services
{
    public class ContractStatusObserverService : IContractObserver
    {
        private readonly TechMoveDbContext context;

        public ContractStatusObserverService(TechMoveDbContext context)
        {
            this.context = context;
        }

        public async Task UpdateContractStatusAsync(Contract contract)
        {
            if (contract.EndDate < DateOnly.FromDateTime(DateTime.Now))
            {
                contract.Status = "Expired";

                await context.SaveChangesAsync();
            }
        }
    }
}
