using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using TechMove.Models.DTOs;

namespace TechMove.Controllers
{
    public class ClientPortalController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public ClientPortalController(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        public async Task<IActionResult> Index()
        {
            var client = _httpClientFactory.CreateClient("TechMoveBackend");

            var clients = await client.GetFromJsonAsync<List<AllClientsFrontendDto>>("api/clients");

            ViewBag.Clients = new SelectList(clients, "Id", "Name");

            return View();
        }

        public async Task<IActionResult> Dashboard(int clientId)
        {
            var client = _httpClientFactory.CreateClient("TechMoveBackend");

            var dashboard = await client.GetFromJsonAsync<ClientDashboardFrontendDto>(
                $"api/clientportal/{clientId}/dashboard");

            if (dashboard == null)
            {
                return NotFound();
            }

            return View(dashboard);
        }
    }
    }
