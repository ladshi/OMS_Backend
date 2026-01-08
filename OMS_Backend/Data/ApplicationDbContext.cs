using Microsoft.EntityFrameworkCore;
using OMS_Backend.Entities;

namespace OMS_Backend.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Product> Product { get; set; }
    }
}
