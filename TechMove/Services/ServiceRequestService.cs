using Microsoft.EntityFrameworkCore;
using TechMove.Data;
using TechMove.Interfaces;

namespace TechMove.Services
{
    public class ServiceRequestService : IServiceRequestService
    {
        private readonly TechMoveDbContext context;

        public ServiceRequestService(TechMoveDbContext context)
        {
            this.context = context;
        }

        public async Task<bool> CanCreateRequestAsync(int contractId)
        {
            var contract = await context.Contracts.FindAsync(contractId);

            if (contract == null)
                return false;

            return contract.Status != "Expired"
                && contract.Status != "OnHold";
        }
    }
}
