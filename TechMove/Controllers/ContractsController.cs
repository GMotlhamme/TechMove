using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TechMove.Data;
using TechMove.Models.Domain;
using TechMove.Services;

namespace TechMove.Controllers
{
    public class ContractsController : Controller
    {
        private readonly TechMoveDbContext _context;
        private readonly FileUploadValidationService _fileUploadValidationService;

        public ContractsController(TechMoveDbContext context, FileUploadValidationService fileUploadValidationService)
        {
            _context = context;
            _fileUploadValidationService = fileUploadValidationService;
        }

        // GET: Contracts
        public async Task<IActionResult> Index(
            string? status,
            DateTime? startDate,
            DateTime? endDate)
        {
            var contracts = _context.Contracts.AsQueryable();

            if (status != null)
            {
                contracts = contracts.Where(c => c.Status.Contains(status));//using LINQ we filtered through contracts by status
            }

            if (startDate.HasValue)
            {
                contracts = contracts.Where(c =>
                    c.StartDate >= DateOnly.FromDateTime(startDate.Value));
            }

            if (endDate.HasValue)
            {
                contracts = contracts.Where(c =>
        c.EndDate <= DateOnly.FromDateTime(endDate.Value));
            }

            return View(await contracts.ToListAsync());
        }

        //public async Task<IActionResult> MyContracts(string userName)
        //{
        //    _context.Contracts.FindAsync(userName);
        //}
        // GET: Contracts/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var contract = await _context.Contracts
                .Include(c => c.Client)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (contract == null)
            {
                return NotFound();
            }

            return View(contract);
        }

        // GET: Contracts/Create
        public IActionResult Create()
        {
            ViewData["ClientId"] = new SelectList(_context.Clients, "Id", "Name");
            return View();
        }

        // POST: Contracts/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,StartDate,EndDate,Status,ServiceLevel,SignedAgreement,ClientId")] Contract contract, IFormFile ConfirmedSignedAgreement)
        {
            if (ModelState.IsValid)
            {
                if (ConfirmedSignedAgreement != null && ConfirmedSignedAgreement.Length > 0)
                {
                    try
                    {
                        //uploading the form
                        _fileUploadValidationService.ValidatePdf(ConfirmedSignedAgreement.FileName);
                    }
                    catch (Exception ex)
                    {
                        ModelState.AddModelError("", ex.Message);
                        ViewData["ClientId"] = new SelectList(_context.Clients, "ClientId", "Fullname", contract.Id);
                        return View(contract);
                    }

                    var fileName = Guid.NewGuid().ToString() + ".pdf";

                    var uploadPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/uploads", fileName);

                    using (var stream = new FileStream(uploadPath, FileMode.Create))
                    {
                        await ConfirmedSignedAgreement.CopyToAsync(stream);
                    }

                    contract.SignedAgreement = "/uploads/" + fileName;
                }
                _context.Add(contract);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["ClientId"] = new SelectList(_context.Clients, "Id", "Name", contract.ClientId);
            return View(contract);
        }

        // GET: Contracts/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var contract = await _context.Contracts.FindAsync(id);
            if (contract == null)
            {
                return NotFound();
            }
            ViewData["ClientId"] = new SelectList(_context.Clients, "Id", "Name", contract.ClientId);
            return View(contract);
        }

        // POST: Contracts/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,StartDate,EndDate,Status,ServiceLevel,SignedAgreement,ClientId")] Contract contract)
        {
            if (id != contract.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(contract);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ContractExists(contract.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["ClientId"] = new SelectList(_context.Clients, "Id", "Name", contract.ClientId);
            return View(contract);
        }

        // GET: Contracts/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var contract = await _context.Contracts
                .Include(c => c.Client)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (contract == null)
            {
                return NotFound();
            }

            return View(contract);
        }

        // POST: Contracts/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var contract = await _context.Contracts.FindAsync(id);
            if (contract != null)
            {
                _context.Contracts.Remove(contract);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ContractExists(int id)
        {
            return _context.Contracts.Any(e => e.Id == id);
        }
    }
}
