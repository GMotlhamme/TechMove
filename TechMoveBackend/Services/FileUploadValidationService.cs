namespace TechMove.Services
{
    public class FileUploadValidationService
    {
        public void ValidatePdf(string fileName)
        {
            var extention = Path.GetExtension(fileName).ToLower() ;
            if(extention != ".pdf")
            {
                throw new Exception("Only PDF types are allowed");
            }
        }
    }
}
