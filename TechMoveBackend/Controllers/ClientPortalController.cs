using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using TechMove.Data;
using TechMove.Models.Domain;
using TechMoveBackend.Models.DTOs;

namespace TechMoveBackend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ClientPortalController : ControllerBase
    {
        private readonly TechMoveDbContext _context;
        public ClientPortalController(TechMoveDbContext context)
        {
            _context = context;
        }
        

        [HttpGet]
        [Route("{clientId}/dashboard")]
        public async Task<ActionResult<ClientDashboardDto>> Dashboard(int clientId)
        {
            var client = await _context.Clients.Include(c => c.Contracts).ThenInclude(c => c.ServiceRequests).FirstOrDefaultAsync(c => c.Id == clientId);

            if (client == null)
            {
                return NotFound();
            }

            var dto = new ClientDashboardDto
            {
                Id = client.Id,
                Name = client.Name,
                Region = client.Region,

                Contracts = client.Contracts.Select(c => new ContractDto
                {
                    Id = c.Id,
                    StartDate = c.StartDate,
                    EndDate = c.EndDate,
                    Status = c.Status,
                    ServiceLevel = c.ServiceLevel,
                    SignedAgreement = c.SignedAgreement,

                    ServiceRequests = c.ServiceRequests.Select(sr => new ServiceRequestDto
                    {
                        Id = sr.Id,
                        ServiceName = sr.ServiceName,
                        Description = sr.Description,
                        Cost = sr.Cost,
                        Status = sr.Status
                    }).ToList() ?? []
                }).ToList() ?? []

                
            };

            return Ok(dto);
        }
    }
}
