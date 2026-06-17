namespace TechMoveBackend.Models.DTOs
{
    public class ClientDashboardDto
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string Region { get; set; }

        public List<ContractDto> Contracts { get; set; }
    }
}
