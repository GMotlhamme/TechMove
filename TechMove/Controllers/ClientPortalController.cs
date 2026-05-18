using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using TechMove.Data;

namespace TechMove.Controllers
{
    public class ClientPortalController : Controller
    {
        private readonly TechMoveDbContext _context;
        public ClientPortalController(TechMoveDbContext context)
        {
            _context = context;
        }
        public async Task<IActionResult> Index()
        {
            ViewBag.Clients = new SelectList(
                await _context.Clients.ToListAsync(),
                "Id",
                "Name");

            return View();
        }

        public async Task<IActionResult> Dashboard(int clientId)
        {
            var client = await _context.Clients
                .Include(c => c.Contracts)
                .ThenInclude(c => c.ServiceRequests)
                .FirstOrDefaultAsync(c => c.Id == clientId);

            return View(client);
        }
    }
}
