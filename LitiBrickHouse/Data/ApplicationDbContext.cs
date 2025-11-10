using LitiBrickHouse.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace LitiBrickHouse.Data
{

    public class ApplicationDbContext : IdentityDbContext
    {
        public DbSet<ProductType> ProductTypes { get; set; }
        public DbSet<OptionCategory> OptionCategories { get; set; }
        public DbSet<CustomOption> CustomOptions { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderCustomLego> OrderCustomLegos { get; set; }
        public DbSet<OrderItem> OrderItems { get; set; }
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {

        }
    }
}
