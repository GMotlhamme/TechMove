
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics.Contracts;
using TechMove.Data;
using TechMove.Interfaces;
using TechMove.Models.Domain;
using TechMoveBackend.Models.DTOs;

[ApiController]
[Route("api/[controller]")]
public class ServiceRequestsController : ControllerBase
{
    private readonly TechMoveDbContext _context;
    private readonly IServiceRequestService serviceRequestService;
    private readonly ICurrencyService currencyApiAdapterService;
    private readonly IContractObserver contractObserver;

    public ServiceRequestsController(TechMoveDbContext context, IServiceRequestService serviceRequestService, ICurrencyService currencyApiAdapterService, IContractObserver contractObserver)
    {
        _context = context;
        this.serviceRequestService = serviceRequestService;
        this.currencyApiAdapterService = currencyApiAdapterService;
        this.contractObserver = contractObserver;
    }

    // GET: SERVICEREQUESTS
    [HttpGet]
    public async Task<ActionResult> GetAllServiceRequests()    
    {
        var services = await _context.ServiceRequests.ToListAsync();
        return Ok(services);
    }

    // GET: SERVICEREQUESTS/Details/5
    [HttpGet]
    [Route("{id}")]
    public async Task<ActionResult<ServiceRequest>> GetSingleServiceRequest(int id)
    {
        var servicerequest = await _context.ServiceRequests.FirstOrDefaultAsync(m => m.Id == id);

        if (servicerequest == null)
        {
            return NotFound();
        }

        return Ok(servicerequest);
    }


    // POST: SERVICEREQUESTS/Create
    // To protect from overposting attacks, enable the specific properties you want to bind to.
    // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
    [HttpPost]
    public async Task<ActionResult> CreateServiceRequest([FromBody] CreateServiceRequestDto serviceRequest)
    {
      
            var newServiceRequest = new ServiceRequest
            {
                ServiceName = serviceRequest.ServiceName,
                Description = serviceRequest.Description,
                Cost = serviceRequest.Cost,
                Status = serviceRequest.Status,
                ContractId = serviceRequest.ContractId
            };
            //service request validation
            var contractStatus = await _context.Contracts.FirstOrDefaultAsync(x => x.Id == serviceRequest.ContractId);
            
            if (contractStatus != null)
            {
                await contractObserver.UpdateContractStatusAsync(contractStatus);
            }

            if (!await serviceRequestService.CanCreateRequestAsync(serviceRequest.ContractId))
            {
                return BadRequest( "Cannot create request for expired or on-hold contracts.");
                
            }

            try
            {
                //currency conversion
                var convertedAmount = currencyApiAdapterService.ConvertToZar(serviceRequest.Cost);
                serviceRequest.Cost = await convertedAmount;
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

            _context.ServiceRequests.Add(newServiceRequest);
            await _context.SaveChangesAsync();
            return CreatedAtAction( nameof(GetSingleServiceRequest), new { id = newServiceRequest.Id }, serviceRequest);
       
    }


    // POST: SERVICEREQUESTS/Edit/5
    // To protect from overposting attacks, enable the specific properties you want to bind to.
    // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
    [HttpPatch]
    [Route("{id}")]
    public async Task<ActionResult> EditServiceRequest(int id, [FromBody] UpdateServiceRequestDto dto)
    {
        
        var existingRequest = await _context.ServiceRequests.FindAsync(id);

        if (existingRequest == null)
        {
            return NotFound();
        }
        if (!string.IsNullOrEmpty(dto.ServiceName))
        {
            existingRequest.ServiceName = dto.ServiceName;
        }
        if (!string.IsNullOrEmpty(dto.Description))
        {
            existingRequest.Description = dto.Description;
        }
        
        if (!string.IsNullOrEmpty(dto.Status))
        {
            existingRequest.Status = dto.Status;
        }

        await _context.SaveChangesAsync();

        return NoContent();
    }

    

    // POST: SERVICEREQUESTS/Delete/5
    [HttpDelete]
    [Route("{id}")]
    public async Task<ActionResult> DeleteConfirmed(int id)
    {
        var servicerequest = await _context.ServiceRequests.FindAsync(id);
        if (servicerequest != null)
        {
            _context.ServiceRequests.Remove(servicerequest);
        }

        await _context.SaveChangesAsync();
        return NoContent();
    }

    
}
