using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using TechMove.Models.DTOs;

namespace TechMove.Controllers
{
    public class ClientContractsController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public ClientContractsController(
            IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }
        public async Task<IActionResult> UploadAgreement(int id)
        {

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

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UploadAgreement(int id, IFormFile ConfirmedSignedAgreement)
        {
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

            if (ConfirmedSignedAgreement != null && ConfirmedSignedAgreement.Length > 0 && contract != null)
            {
                using var formData = new MultipartFormDataContent();

                using var fileStream = ConfirmedSignedAgreement.OpenReadStream();

                formData.Add(new StreamContent(fileStream), "ConfirmedSignedAgreement", ConfirmedSignedAgreement.FileName);

                var uploadResponse = await client.PutAsync($"api/clientcontracts/{contract.Id}/agreement", formData);

                if (!uploadResponse.IsSuccessStatusCode)
                {
                    var error = await uploadResponse.Content.ReadAsStringAsync();

                    ModelState.AddModelError( "",$"Agreement upload failed: {error}");
                    return View(contract);
                }
            }

            return RedirectToAction(
                "Dashboard",
                "ClientPortal",
                new { clientId = contract.ClientId });
        }

        public async Task<IActionResult> DownloadAgreement(int id)
        {
            var client = _httpClientFactory.CreateClient("TechMoveBackend");

            var response = await client.GetAsync(
                $"api/clientcontracts/{id}/agreement/download");

            if (!response.IsSuccessStatusCode)
            {
                return NotFound();
            }

            var fileBytes = await response.Content.ReadAsByteArrayAsync();

            var fileName = response.Content.Headers.ContentDisposition?.FileName?.Trim('"')
                ?? "SignedAgreement.pdf";

            return File(fileBytes, "application/pdf", fileName);
        }
    }
}
