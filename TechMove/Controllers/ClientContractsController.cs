using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using TechMove.Data;
using TechMove.Services;

namespace TechMove.Controllers
{
    public class ClientContractsController : Controller
    {
        private readonly TechMoveDbContext _context;
        private readonly FileUploadValidationService _fileUploadValidationService;

        public ClientContractsController(TechMoveDbContext context, FileUploadValidationService fileUploadValidationService)
        {
            _context = context;
            _fileUploadValidationService = fileUploadValidationService;
        }

        public async Task<IActionResult> UploadAgreement(int id)
        {
            var contract = await _context.Contracts.FindAsync(id);

            if (contract == null)
            {
                return NotFound();
            }

            return View(contract);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UploadAgreement(int id, IFormFile ConfirmedSignedAgreement)
        {
            var contract = await _context.Contracts.FindAsync(id);

            if (contract == null)
            {
                return NotFound();
            }

            if (ConfirmedSignedAgreement != null &&
                ConfirmedSignedAgreement.Length > 0)
            {
                try
                {
                    _fileUploadValidationService
                        .ValidatePdf(ConfirmedSignedAgreement.FileName);

                    var fileName = Guid.NewGuid() + ".pdf";

                    var uploadPath = Path.Combine(
                        Directory.GetCurrentDirectory(),
                        "wwwroot/uploads",
                        fileName);

                    using (var stream = new FileStream(uploadPath, FileMode.Create))
                    {
                        await ConfirmedSignedAgreement.CopyToAsync(stream);
                    }

                    contract.SignedAgreement = "/uploads/" + fileName;

                    await _context.SaveChangesAsync();
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", ex.Message);

                    return View(contract);
                }
            }

            return RedirectToAction(
                "Dashboard",
                "ClientPortal",
                new { clientId = contract.ClientId });
        }
    }
}
