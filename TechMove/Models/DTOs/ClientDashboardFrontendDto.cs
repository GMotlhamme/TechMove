namespace TechMove.Models.DTOs
{
    public class ClientDashboardFrontendDto
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string Region { get; set; }

        public List<ClientContractFrontendDto> Contracts { get; set; } = new();
    }
}
