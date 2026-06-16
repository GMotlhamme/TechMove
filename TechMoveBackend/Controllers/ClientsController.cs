using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Threading.Tasks;
using TechMove.Data;
using TechMove.Models.Domain;
using TechMoveBackend.Models.DTOs;

namespace TechMoveBackend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ClientsController : ControllerBase
    {
        private readonly TechMoveDbContext _context;

        public ClientsController(TechMoveDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<Client>> GetAllCLients()
        {
            var clientList = await _context.Clients.ToListAsync();

            return Ok(clientList);
        }

        // GET: Clients/Details/5
        [HttpGet]
        [Route("{id}")]
        public async Task<ActionResult> GetSingleClient(int id)
        {

            var client = await _context.Clients.FirstOrDefaultAsync(m => m.Id == id);

            if (client == null)
            {
                return NotFound();
            }

            return Ok(client);
        }

        

        // POST: Clients/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        public async Task<ActionResult> CreateClient([FromBody] CreateClientDto dto)
        {
            var client = new Client
            {
                Name = dto.Name,
                ContractDetails = dto.ContractDetails,
                Region = dto.Region
            };

            if (!ModelState.IsValid)
            {
                return BadRequest();
            }

            _context.Clients.Add(client);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetSingleClient), new { id = client.Id }, client);
        }


        // POST: Clients/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPatch]
        public async Task<ActionResult> Edit(int id, [FromBody] CreateClientDto dto)
        {
            

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var client = await _context.Clients.FindAsync(id);

            if (client == null)
            {
                return NotFound();
            }

            if (!string.IsNullOrEmpty(dto.Name))
            {
                client.Name = dto.Name;
            }

            if (!string.IsNullOrEmpty(dto.ContractDetails))
            {
                client.ContractDetails = dto.ContractDetails;
            }

            if (!string.IsNullOrEmpty(dto.Region))
            {
                client.Region = dto.Region;
            }

            await _context.SaveChangesAsync();

            return Ok(client);
        }

        

        // POST: Clients/Delete/5
        [HttpDelete]
        [Route("{id}")]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            
            var client = await _context.Clients.FindAsync(id);
            
            if (client == null)
            {
                return NotFound();
            }

            _context.Clients.Remove(client);
            await _context.SaveChangesAsync();
            return Ok();
        }

        
    }
}
