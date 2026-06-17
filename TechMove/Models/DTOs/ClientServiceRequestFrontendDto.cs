namespace TechMove.Models.DTOs
{
    public class ClientServiceRequestFrontendDto
    {
        public int Id { get; set; }

        public string ServiceName { get; set; }

        public string Description { get; set; }

        public decimal Cost { get; set; }

        public string Status { get; set; }
    }
}
