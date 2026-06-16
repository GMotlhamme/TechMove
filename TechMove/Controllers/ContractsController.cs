using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Build.Tasks;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TechMove.Models.DTOs;
namespace TechMove.Controllers
{
    public class ContractsController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public ContractsController(
            IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        // GET: Contracts
        public async Task<IActionResult> Index(
            string? status,
            DateTime? startDate,
            DateTime? endDate)
        {
            var client = _httpClientFactory.CreateClient("TechMoveBackend");

            var response = await client.GetAsync($"api/Contracts?status={status}&startDate={startDate:yyyy-MM-dd}&endDate={endDate:yyyy-MM-dd}");

            if (!response.IsSuccessStatusCode)
            {
                return View(new List<ContractsFrontendDto>());
            }

            var contracts = await response.Content.ReadFromJsonAsync<List<ContractsFrontendDto>>();

            return View(contracts);
        }

     
        //GET: Contracts/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var client = _httpClientFactory.CreateClient("TechMoveBackend");
            var response = await client.GetAsync($"api/Contracts/{id}");

            if (!response.IsSuccessStatusCode)
            {
                return View(new SingleContractFrontendDto());
            }

            var contract = await response.Content.ReadFromJsonAsync<SingleContractFrontendDto>();

            if (contract == null)
            {
                return NotFound();
            }

            return View(contract);
        }

        //helper method to get dropdown of clients
        private async Task PopulateClientsDropDown(HttpClient client, int? selectedClientId = null)
        {
            var clients =await client.GetFromJsonAsync<List<AllClientsFrontendDto>>("api/clients");

            ViewData["ClientId"] = new SelectList(clients, "Id", "Name", selectedClientId);
        }
        // GET: Contracts/Create
        public async Task<IActionResult> Create()
        {
            var client = _httpClientFactory.CreateClient("TechMoveBackend");

            await PopulateClientsDropDown(client);

            return View();
        }

        //// POST: Contracts/Create
        //// To protect from overposting attacks, enable the specific properties you want to bind to.
        //// For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,StartDate,EndDate,Status,ServiceLevel,SignedAgreement,ClientId")] CreateContractFrontendDto contract, IFormFile ConfirmedSignedAgreement)
        {
            if (!ModelState.IsValid)
            {
                var client = _httpClientFactory.CreateClient("TechMoveBackend");
                await PopulateClientsDropDown(client, contract.ClientId);
                return View(contract);
            }

            var apiClient = _httpClientFactory.CreateClient("TechMoveBackend");

            var response = await apiClient.PostAsJsonAsync("api/contracts", new
            {
                StartDate = contract.StartDate,
                EndDate = contract.EndDate,
                Status = contract.Status,
                ServiceLevel = contract.ServiceLevel,
                ClientId = contract.ClientId
            });

            if (!response.IsSuccessStatusCode)
            {
                ModelState.AddModelError("", "Failed to create contract.");
                await PopulateClientsDropDown(apiClient, contract.ClientId);
                return View(contract);
            }

            var createdContract = await response.Content.ReadFromJsonAsync<CreateContractFrontendDto>();

            if (ConfirmedSignedAgreement != null && ConfirmedSignedAgreement.Length > 0 && createdContract != null)
            {
                using var formData = new MultipartFormDataContent();

                using var fileStream = ConfirmedSignedAgreement.OpenReadStream();

                formData.Add(new StreamContent(fileStream), "confirmedSignedAgreement", ConfirmedSignedAgreement.FileName);

                var uploadResponse = await apiClient.PutAsync($"api/clientcontracts/{createdContract.Id}/agreement",formData);

                if (!uploadResponse.IsSuccessStatusCode)
                {
                    ModelState.AddModelError("", "Contract created, but agreement upload failed. ");

                    await PopulateClientsDropDown(apiClient, contract.ClientId);
                    return View(contract);
                }
            }

            return RedirectToAction(nameof(Index));
        }

        //// GET: Contracts/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var client = _httpClientFactory.CreateClient("TechMoveBackend");

            var contract = await client.GetFromJsonAsync<SingleContractFrontendDto>($"api/contracts/{id}");

            if (contract == null)
            {
                return NotFound();
            }

            var model = new UpdateContractStatusFrontendDto
            {
                Id = contract.Id,
                Status = contract.Status
            };

            return View(model);
        }

        //// POST: Contracts/Edit/5
        //// To protect from overposting attacks, enable the specific properties you want to bind to.
        //// For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,StartDate,EndDate,Status,ServiceLevel,SignedAgreement,ClientId")] UpdateContractStatusFrontendDto model)
        {
            if (id != model.Id)
            {
                return NotFound();
            }

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var client = _httpClientFactory.CreateClient("TechMoveBackend");

            var response = await client.PatchAsJsonAsync( $"api/contracts/{id}/status", new { status = model.Status });

            if (response.IsSuccessStatusCode)
            {
                return RedirectToAction(nameof(Index));
            }

            ModelState.AddModelError("", "Failed to update contract status.");

            return View(model);
        }

        // GET: Contracts/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var client = _httpClientFactory.CreateClient("TechMoveBackend");

            var contract = await client.GetFromJsonAsync<SingleContractFrontendDto>($"api/contracts/{id}");
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
            var client = _httpClientFactory.CreateClient("TechMoveBackend");

            var contract = await client.GetFromJsonAsync<SingleContractFrontendDto>($"api/Contracts/{id}");
            
            if (contract != null)
            {
                await client.DeleteAsync($"api/contracts/{id}");
            }

            return RedirectToAction(nameof(Index));
        }

        
    }
}
