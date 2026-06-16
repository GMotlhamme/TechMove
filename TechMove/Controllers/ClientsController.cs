using Azure;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TechMove.Models.DTOs;

namespace TechMove.Controllers
{
    public class ClientsController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public ClientsController(
            IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        // GET: Clients
        public async Task<IActionResult> Index()
        {
            var client = _httpClientFactory.CreateClient("TechMoveBackend");

            var response = await client.GetAsync($"api/Clients");

            if (!response.IsSuccessStatusCode)
            {
                return View(new List<AllClientsFrontendDto>());
            }

            var contracts = await response.Content.ReadFromJsonAsync<List<AllClientsFrontendDto>>();
            return View(contracts);
        }

        // GET: Clients/Details/5
                public async Task<IActionResult> Details(int? id)
        {
            var apiClient = _httpClientFactory.CreateClient("TechMoveBackend");

            var response = await apiClient.GetAsync($"api/Clients/{id}");

            if (!response.IsSuccessStatusCode)
            {
                return View(new AllClientsFrontendDto());
            }

            var client = await response.Content.ReadFromJsonAsync<AllClientsFrontendDto>();

            if (client == null)
            {
                return NotFound();
            }

            return View(client);
        }

        //        // GET: Clients/Create
                public IActionResult Create()
                {
                    return View();
                }

               // POST: Clients/Create
               // To protect from overposting attacks, enable the specific properties you want to bind to.
               // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
            [HttpPost]
            [ValidateAntiForgeryToken]
            public async Task<IActionResult> Create([Bind("Id,Name,ContractDetails,Region")] AllClientsFrontendDto client)
            {
                if (ModelState.IsValid)
                {
                    var apiClient = _httpClientFactory.CreateClient("TechMoveBackend");
                    var response = await apiClient.PostAsJsonAsync($"api/Clients", new {Name = client.Name, ContractDetails = client.ContractDetails, Region = client.Region});

                    if (!response.IsSuccessStatusCode)
                    {
                        ModelState.AddModelError("", "Failed to create client.");
                        return View(client);

                    }
                    return RedirectToAction(nameof(Index));
                }

                return View(client);
            }

        //        // GET: Clients/Edit/5
                public async Task<IActionResult> Edit(int? id)
                {
                    if (id == null)
                    {
                        return NotFound();
                    }
                    var apiClient = _httpClientFactory.CreateClient("TechMoveBackend");

                    var client = await apiClient.GetFromJsonAsync<AllClientsFrontendDto>($"api/Clients/{id}");
                    if (client == null)
                    {
                        return NotFound();
                    }
                    return View(client);
                }

                // POST: Clients/Edit/5
                // To protect from overposting attacks, enable the specific properties you want to bind to.
                // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
                [HttpPost]
                [ValidateAntiForgeryToken]
                public async Task<IActionResult> Edit(int id, [Bind("Id,Name,ContractDetails,Region")] AllClientsFrontendDto client)
                {
                    if (id != client.Id)
                    {
                        return NotFound();
                    }

                    if (!ModelState.IsValid)
                    {
                         return View(client);
                    }
                        var apiClient = _httpClientFactory.CreateClient("TechMoveBackend");

                        var response = await apiClient.PatchAsJsonAsync($"api/Clients/{id}", new { Name = client.Name, ContractDetails = client.ContractDetails, Region = client.Region });
                    
                    if (response.IsSuccessStatusCode)
                    {
                        return RedirectToAction(nameof(Index));
                    }

                    ModelState.AddModelError("", "Failed to update client details.");
                    return View(client);
                }

        // GET: Clients/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var apiClient = _httpClientFactory.CreateClient("TechMoveBackend");

            var client = await apiClient.GetFromJsonAsync<AllClientsFrontendDto>($"api/Clients/{id}");
        
            if (client == null)
            {
                return NotFound();
            }

            return View(client);
        }

        // POST: Clients/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var apiClient = _httpClientFactory.CreateClient("TechMoveBackend");

            var client = await apiClient.GetFromJsonAsync<AllClientsFrontendDto>($"api/Clients/{id}");

            if (client != null)
            {
                await apiClient.DeleteAsync($"api/Clients/{id}");
            }

            return RedirectToAction(nameof(Index));
        }


    }
}
