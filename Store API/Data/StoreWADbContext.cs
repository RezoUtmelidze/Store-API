using Microsoft.EntityFrameworkCore;
using Store_API.Data.Config;

namespace Store_API.Data
{
    public class StoreWADbContext : DbContext
    {
        public StoreWADbContext(DbContextOptions<StoreWADbContext> options) : base(options) { }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new StoreConfig());
            modelBuilder.ApplyConfiguration(new EmployeeConfig());
            modelBuilder.ApplyConfiguration(new CategoryConfig());
            modelBuilder.ApplyConfiguration(new ProductConfig());
        }
        public DbSet<Store> Stores { get; set; }
        public DbSet<Employee> Employees { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Product> Products { get; set; }
    }
}
