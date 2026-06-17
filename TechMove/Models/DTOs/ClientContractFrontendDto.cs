namespace TechMove.Models.DTOs
{
    public class ClientContractFrontendDto
    {
        public int Id { get; set; }

        public DateOnly StartDate { get; set; }

        public DateOnly EndDate { get; set; }

        public string Status { get; set; }

        public string ServiceLevel { get; set; }

        public string? SignedAgreement { get; set; }

        public List<ClientServiceRequestFrontendDto> ServiceRequests {  get;  set; } = new();
    }
}
