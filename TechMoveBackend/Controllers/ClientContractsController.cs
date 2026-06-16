using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using TechMove.Data;
using TechMove.Models.Domain;
using TechMove.Services;

namespace TechMove.Controllers
{
    [ApiController]
    [Route("/api/[controller]")]
    public class ClientContractsController : ControllerBase
    {
        private readonly TechMoveDbContext _context;
        private readonly FileUploadValidationService _fileUploadValidationService;

        public ClientContractsController(TechMoveDbContext context, FileUploadValidationService fileUploadValidationService)
        {
            _context = context;
            _fileUploadValidationService = fileUploadValidationService;
        }
        [HttpGet]
        public async Task<ActionResult<Contract>> UploadAgreement(int id)
        {
            var contract = await _context.Contracts.FindAsync(id);

            if (contract == null)
            {
                return NotFound();
            }

            return contract;
        }

        [HttpPut]
        public async Task<ActionResult> UploadAgreement(int id, IFormFile ConfirmedSignedAgreement)
        {
            var contract = await _context.Contracts.FindAsync(id);

            if (contract == null)
            {
                return NotFound();
            }

            
                    _fileUploadValidationService.ValidatePdf(ConfirmedSignedAgreement.FileName);

                    var fileName = Guid.NewGuid() + ".pdf";

                    var uploadPath = Path.Combine(
                        Directory.GetCurrentDirectory(),
                        "SavedDocuments",
                        fileName);

                    using (var stream = new FileStream(uploadPath, FileMode.Create))
                    {
                        await ConfirmedSignedAgreement.CopyToAsync(stream);
                    }

                    contract.SignedAgreement = "/uploads/" + fileName;

                    await _context.SaveChangesAsync();
                
            

            return Ok(contract);
        }
    }
}
