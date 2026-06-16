namespace TechMoveBackend.Models.DTOs
{
    public class CreateClientDto
    {
        public string Name { get; set; } = string.Empty;
        public string ContractDetails { get; set; } = string.Empty;
        public string Region { get; set; } = string.Empty;
    }
}
