
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics.Contracts;
using TechMove.Data;
using TechMove.Interfaces;
using TechMove.Models.Domain;

public class ServiceRequestsController : Controller
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
    public async Task<IActionResult> Index()    
    {
        return View(await _context.ServiceRequests.ToListAsync());
    }

    // GET: SERVICEREQUESTS/Details/5
    public async Task<IActionResult> Details(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var servicerequest = await _context.ServiceRequests
            .FirstOrDefaultAsync(m => m.Id == id);
        if (servicerequest == null)
        {
            return NotFound();
        }

        return View(servicerequest);
    }

    // GET: SERVICEREQUESTS/Create
    public async Task<IActionResult> CreateAsync()
    {
        ViewBag.Contracts = new SelectList(
        await _context.Contracts
        .Select(c => new
        {
            c.Id,
            Display = $"Contract #{c.Id} - {c.ServiceLevel}"
        })
        .ToListAsync(),
    "Id",
    "Display");
        return View();
    }

    // POST: SERVICEREQUESTS/Create
    // To protect from overposting attacks, enable the specific properties you want to bind to.
    // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([Bind("Id,ServiceName,Description,Cost,Status,ContractId,Contract")] ServiceRequest servicerequest)
    {
        if (ModelState.IsValid)
        {
            //service request validation
            var contractStatus = await _context.Contracts.FirstOrDefaultAsync(x => x.Id == servicerequest.ContractId);
            if (contractStatus != null)
            {
                await contractObserver.UpdateContractStatusAsync(contractStatus);
            }
            if (!await serviceRequestService.CanCreateRequestAsync(servicerequest.ContractId))
            {
                ModelState.AddModelError("", "Cannot create request for expired or on-hold contracts.");
                return View(servicerequest);
            }


            try
            {
                //currency conversion
                var convertedAmount = currencyApiAdapterService.ConvertToZar(servicerequest.Cost);
                servicerequest.Cost = await convertedAmount;
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                return View(servicerequest);
            }

            _context.Add(servicerequest);
            await _context.SaveChangesAsync();
            return RedirectToAction("Dashboard", "ClientPortal",new { clientId = contractStatus.ClientId });
        }
        return View(servicerequest);
    }

    // GET: SERVICEREQUESTS/Edit/5
    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var servicerequest = await _context.ServiceRequests.FindAsync(id);
        if (servicerequest == null)
        {
            return NotFound();
        }
        return View(servicerequest);
    }

    // POST: SERVICEREQUESTS/Edit/5
    // To protect from overposting attacks, enable the specific properties you want to bind to.
    // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int? id, [Bind("Id,ServiceName,Description,Cost,Status,ContractId,Contract")] ServiceRequest servicerequest)
    {
        if (id != servicerequest.Id)
        {
            return NotFound();
        }

        if (ModelState.IsValid)
        {
            try
            {
                _context.Update(servicerequest);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ServiceRequestExists(servicerequest.Id))
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
        return View(servicerequest);
    }

    // GET: SERVICEREQUESTS/Delete/5
    public async Task<IActionResult> Delete(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var servicerequest = await _context.ServiceRequests
            .FirstOrDefaultAsync(m => m.Id == id);
        if (servicerequest == null)
        {
            return NotFound();
        }

        return View(servicerequest);
    }

    // POST: SERVICEREQUESTS/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int? id)
    {
        var servicerequest = await _context.ServiceRequests.FindAsync(id);
        if (servicerequest != null)
        {
            _context.ServiceRequests.Remove(servicerequest);
        }

        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    private bool ServiceRequestExists(int? id)
    {
        return _context.ServiceRequests.Any(e => e.Id == id);
    }
}
