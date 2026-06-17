using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using TechMove.Data;
using TechMove.Models.Domain;
using TechMove.Services;
using TechMoveBackend.Models.DTOs;

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
        [Route("{id}")]
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
        [Route("{id}/agreement")]
        public async Task<ActionResult> UploadAgreement([FromRoute] int id, UploadAgreementDto dto )
        {
            var contract = await _context.Contracts.FindAsync(id);

            if (contract == null)
            {
                return NotFound();
            }

            if (dto.ConfirmedSignedAgreement == null || dto.ConfirmedSignedAgreement.Length == 0)
            {
                return BadRequest("No file was uploaded.");
            }

            _fileUploadValidationService.ValidatePdf(dto.ConfirmedSignedAgreement.FileName);

            var fileName = Guid.NewGuid() + ".pdf";

            var folderPath = Path.Combine(Directory.GetCurrentDirectory(), "SavedDocuments");

            Directory.CreateDirectory(folderPath);

            var uploadPath = Path.Combine(folderPath, fileName);

            using (var stream = new FileStream(uploadPath, FileMode.Create))
            {
                await dto.ConfirmedSignedAgreement.CopyToAsync(stream);
            }

            contract.SignedAgreement = "/SavedDocuments/" + fileName;

            await _context.SaveChangesAsync();

            return Ok(contract);
        }


        [HttpGet("{id}/agreement/download")]
        public async Task<IActionResult> DownloadAgreement(int id)
        {
            var contract = await _context.Contracts.FindAsync(id);

            if (contract == null || string.IsNullOrEmpty(contract.SignedAgreement))
            {
                return NotFound();
            }

            var fileName = Path.GetFileName(contract.SignedAgreement);

            var filePath = Path.Combine( Directory.GetCurrentDirectory(), "SavedDocuments", fileName);

            if (!System.IO.File.Exists(filePath))
            {
                return NotFound();
            }

            var fileBytes = await System.IO.File.ReadAllBytesAsync(filePath);

            return File(fileBytes, "application/pdf", fileName);
        }
    }
}
