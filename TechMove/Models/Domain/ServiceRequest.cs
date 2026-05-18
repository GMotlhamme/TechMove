using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TechMove.Models.Domain
{
    
    public class ServiceRequest
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string ServiceName { get; set; }

        [Required]
        public string Description { get; set; }

        [Column(TypeName = "decimal(18,3)")]
        public decimal Cost { get; set; }

        [Required]
        public string? Status { get; set; }

        public int ContractId { get; set; }

        [ForeignKey("ContractId")]
        public Contract? Contract { get; set; }
    }
}
