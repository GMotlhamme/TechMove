namespace TechMove.Interfaces
{
    public interface IServiceRequestService
    {
        Task<bool> CanCreateRequestAsync(int contractId);//update method contract for the observer pattern
    }
}
