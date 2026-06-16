
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics.Contracts;
using TechMove.Models.DTOs;

public class ServiceRequestsController : Controller
{
    private readonly IHttpClientFactory _httpClientFactory;

    public ServiceRequestsController(
        IHttpClientFactory httpClientFactory)
    {
        _httpClientFactory = httpClientFactory;
    }

    // GET: SERVICEREQUESTS
    public async Task<IActionResult> Index()    
    {
        var client = _httpClientFactory.CreateClient("TechMoveBackend");

        var response = await client.GetAsync($"api/ServiceRequests");
        if (!response.IsSuccessStatusCode)
        {
            return View(new List<AllServiceRequestsFrontendDto>());
        }

        var serviceRequests = await response.Content.ReadFromJsonAsync<List<AllServiceRequestsFrontendDto>>();

        return View(serviceRequests);
    }

    // GET: SERVICEREQUESTS/Details/5
    public async Task<IActionResult> Details(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var client = _httpClientFactory.CreateClient("TechMoveBackend");
        var response = await client.GetAsync($"api/servicerequests/{id}");

        if (!response.IsSuccessStatusCode)
        {
            return View(new AllServiceRequestsFrontendDto());
        }

        var servicerequest = await response.Content.ReadFromJsonAsync<AllServiceRequestsFrontendDto>();

        if (servicerequest == null)
        {
            return NotFound();
        }

        return View(servicerequest);
    }

    private async Task PopulateContractsDropDown(int? selectedContractId = null)
    {
        var client = _httpClientFactory.CreateClient("TechMoveBackend");

        var contracts = await client.GetFromJsonAsync<List<ContractsFrontendDto>>("api/contracts");

        ViewBag.Contracts = new SelectList(contracts, "Id", "ServiceLevel", selectedContractId);
    }

    // GET: SERVICEREQUESTS/Create
    public async Task<IActionResult> Create()
    {
        var client = _httpClientFactory.CreateClient("TechMoveBackend");

        var contracts = await client.GetFromJsonAsync<List<ContractsFrontendDto>>( "api/contracts");

        ViewBag.Contracts = new SelectList(  contracts, "Id", "ServiceLevel");

        return View();
    }

    // POST: SERVICEREQUESTS/Create
    // To protect from overposting attacks, enable the specific properties you want to bind to.
    // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(AllServiceRequestsFrontendDto serviceRequest)
    {
        if (!ModelState.IsValid)
        {
            await PopulateContractsDropDown(serviceRequest.ContractId);
            return View(serviceRequest);
        }

        var client = _httpClientFactory.CreateClient("TechMoveBackend");

        var response = await client.PostAsJsonAsync( "api/servicerequests",
            new
            {
                ServiceName = serviceRequest.ServiceName,
                Description = serviceRequest.Description,
                Cost = serviceRequest.Cost,
                Status = serviceRequest.Status,
                ContractId = serviceRequest.ContractId
            });

        if (response.IsSuccessStatusCode)
        {
            var contractResponse = await client.GetAsync($"api/contracts/{serviceRequest.ContractId}");

            if (!contractResponse.IsSuccessStatusCode)
            {
                return RedirectToAction(nameof(Index));
            }

            var contract = await contractResponse.Content.ReadFromJsonAsync<SingleContractFrontendDto>();

            if (contract == null)
            {
                return RedirectToAction(nameof(Index));
            }

            return RedirectToAction("Dashboard", "ClientPortal", new { clientId = contract.ClientId });
        }

        return RedirectToAction(nameof(Index));
    }

    // GET: SERVICEREQUESTS/Edit/5
    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var apiClient = _httpClientFactory.CreateClient("TechMoveBackend");

        var servicerequest = await apiClient.GetFromJsonAsync<AllServiceRequestsFrontendDto>($"api/servicerequests/{id}");
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
    public async Task<IActionResult> Edit(int? id, [Bind("Id,ServiceName,Description,Cost,Status,ContractId,Contract")] AllServiceRequestsFrontendDto servicerequest)
    {
        if (id != servicerequest.Id)
        {
            return NotFound();
        }

        if (!ModelState.IsValid)
        {
            return View(servicerequest);
        }
        var apiClient = _httpClientFactory.CreateClient("TechMoveBackend");

        var response = await apiClient.PatchAsJsonAsync($"api/ServiceRequests/{id}", new
        {
             ServiceName = servicerequest.ServiceName,
             Description = servicerequest.Description,
             Cost = servicerequest.Cost,
             Status = servicerequest.Status , 
             ContractId = servicerequest.ContractId
        });
        if (response.IsSuccessStatusCode)
        {
            return RedirectToAction(nameof(Index));
        }
        var error = await response.Content.ReadAsStringAsync();
        ModelState.AddModelError("", $"Failed to update service request: {error}");
        return View(servicerequest);
    }

    // GET: SERVICEREQUESTS/Delete/5
    public async Task<IActionResult> Delete(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var client = _httpClientFactory.CreateClient("TechMoveBackend");

        var serviceRequest = await client.GetFromJsonAsync<AllServiceRequestsFrontendDto>( $"api/ServiceRequests/{id}");

        if (serviceRequest == null)
        {
            return NotFound();
        }

        return View(serviceRequest);
    }

    // POST: SERVICEREQUESTS/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int? id)
    {
        var client = _httpClientFactory.CreateClient("TechMoveBackend");

        var response = await client.DeleteAsync($"api/ServiceRequests/{id}");

        if (!response.IsSuccessStatusCode)
        {
            ModelState.AddModelError( "","Failed to delete service request.");
            return View();
        }

        return RedirectToAction(nameof(Index));
    }


}
