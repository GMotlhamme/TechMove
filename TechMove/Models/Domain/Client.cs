using System.ComponentModel.DataAnnotations;

namespace TechMove.Models.Domain
{
    public class Client
    {
        [Required]
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        public string ContractDetails { get; set; }

        [Required]
        public string Region { get; set; }

        public List<Contract>? Contracts { get; set; }
    }
}