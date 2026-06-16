namespace TechMoveBackend.Models.DTOs
{
    public class CreateServiceRequestDto
    {
        public string ServiceName { get; set; }
        public string Description { get; set; }
        public decimal Cost { get; set; }
        public string Status { get; set; }
        public int ContractId { get; set; }
    }
}
