namespace TechMoveBackend.Models.DTOs
{
    public class CreateContractDto
    {
        public DateOnly StartDate { get; set; }

        public DateOnly EndDate { get; set; }

        public string Status { get; set; }

        public string ServiceLevel { get; set; }

        public int ClientId { get; set; }
    }
}
