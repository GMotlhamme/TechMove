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
using TechMoveBackend.Models.DTOs;

namespace TechMoveBackend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ContractsController : ControllerBase
    {
        private readonly TechMoveDbContext _context;
        private readonly FileUploadValidationService _fileUploadValidationService;

        public ContractsController(TechMoveDbContext context, FileUploadValidationService fileUploadValidationService)
        {
            _context = context;
            _fileUploadValidationService = fileUploadValidationService;
        }

        // GET: Contracts
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Contract>>> GetAllContracts(
            string? status,
            DateTime? startDate,
            DateTime? endDate)
        {
            var contracts = _context.Contracts.AsQueryable();

            if (status != null)
            {
                contracts = contracts.Where(c => c.Status.Contains(status));
            }

            if (startDate.HasValue)
            {
                contracts = contracts.Where(c => c.StartDate >= DateOnly.FromDateTime(startDate.Value));
            }

            if (endDate.HasValue)
            {
                contracts = contracts.Where(c => c.EndDate <= DateOnly.FromDateTime(endDate.Value));
            }

            return Ok(await contracts.ToListAsync());
        }
         
        // GET: Contracts/Details/5
        [HttpGet("{id}")]
        public async Task<ActionResult> GetSingleContract(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var contract = await _context.Contracts.FirstOrDefaultAsync(m => m.Id == id);
            if (contract == null)
            {
                return NotFound();
            }

            return Ok(contract);
        }

      

        // POST: Contracts/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        public async Task<ActionResult> CreateContract([FromForm] CreateContractDto dto)
        {
            if (!ModelState.IsValid)
            { 
                return BadRequest();
            }
            var contract = new Contract
            {
                StartDate = dto.StartDate,
                EndDate = dto.EndDate,
                Status = dto.Status,
                ServiceLevel = dto.ServiceLevel,
                ClientId = dto.ClientId
            };
            _context.Contracts.Add(contract);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetSingleContract), new { id = contract.Id }, contract);
        }

        // POST: Contracts/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        [HttpPatch]
        [Route("{id}/status")]
        public async Task<ActionResult> UpdateContractStatus(int id,[FromBody] UpdateContractStatusDto dto)
        {

            var contract = await _context.Contracts.FindAsync(id);

            if (contract == null)
            {
                return NotFound();
            }

            contract.Status = dto.Status;

            await _context.SaveChangesAsync();

            return NoContent();

        }

        // GET: Contracts/Delete/5
        [HttpDelete]
        [Route("{id}")]
        public async Task<ActionResult> DeleteContract(int? id)
        {
            var contract = await _context.Contracts.FirstOrDefaultAsync(m => m.Id == id);
            if (contract == null)
            {
                return NotFound();
            }
            if (contract != null)
            {
                _context.Contracts.Remove(contract);
                await _context.SaveChangesAsync();
            }

            return Ok();
        }
    }
}
