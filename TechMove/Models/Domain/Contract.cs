using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TechMove.Models.Domain
{
    //status of the contract, set to enumerable values to avoid them being changed to unexpected values
    public enum contractStatus
    {
        Draft, 
        Active, 
        Expired,
        OnHold
    }
    public class Contract
    {
        [Required]
        public int Id { get; set; }

        public DateOnly StartDate { get; set; }

        public DateOnly EndDate { get; set; }

        public contractStatus Status { get; set; } = contractStatus.Draft;

        [Required]
        public string ServiceLevel { get; set; }

        public string? SignedAgreement { get; set; }

        public int ClientId { get; set; }

        [ForeignKey("ClientId")]
        public Client Client { get; set; }

        public List<ServiceRequest>? ServiceRequests { get; set; }
    }
}
