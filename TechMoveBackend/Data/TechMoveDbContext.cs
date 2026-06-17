using Microsoft.EntityFrameworkCore;
using TechMove.Models.Domain;

namespace TechMove.Data
{
    public class TechMoveDbContext : DbContext
    {
        public TechMoveDbContext(DbContextOptions<TechMoveDbContext> options) : base(options) { }

        public DbSet<Client> Clients { get; set; }

        public DbSet<Contract> Contracts { get; set; }

        public DbSet<ServiceRequest> ServiceRequests { get; set; }
    }
}
