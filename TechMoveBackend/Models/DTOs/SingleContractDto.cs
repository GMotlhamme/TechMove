namespace TechMoveBackend.Models.DTOs
{
    public class SingleContractDto
    {
        public int Id { get; set; }
        public DateOnly StartDate { get; set; }
        public DateOnly EndDate { get; set; }

        public string Status { get; set; }
        public string ServiceLevel { get; set; }
        public string SignedAgreement { get; set; }
        public int ClientId { get; set; }
    }
}
