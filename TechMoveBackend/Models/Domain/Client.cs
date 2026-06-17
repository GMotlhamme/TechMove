using System.ComponentModel.DataAnnotations;

namespace TechMove.Models.Domain
{
    public class Client
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

      
        public string? ContractDetails { get; set; }

        [Required]
        public string Region { get; set; }

        public List<Contract>? Contracts { get; set; }
    }
}