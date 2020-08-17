using Microsoft.EntityFrameworkCore;

namespace ProductsAndCategories.Models
{
    public class PacContext : DbContext
    {
        public PacContext(DbContextOptions options) : base(options) { }
        public DbSet<Product> Products {get;set;}
        public DbSet<Category> Categories {get;set;}
        public DbSet<Association> Associations {get;set;}
    }
}