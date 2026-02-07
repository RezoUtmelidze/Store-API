using Microsoft.EntityFrameworkCore;
using Store_API.Data;

namespace Store_API.Repositories
{
    public class ProductRepos : StoreAPIRepos<Product>, IProductRepos
    {
        private readonly StoreWADbContext _dbContext;
        public ProductRepos(StoreWADbContext dbContext) : base(dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<List<Product>> GetAllProductsWithCategoriesAsync()
        {
            return await _dbContext.Products.Include(p => p.Categories).ToListAsync();
        }

        public async Task<List<Product>> GetProductsAsync(Func<Product, bool> filter)
        {
            return await _dbContext.Products.Where(p => filter(p)).ToListAsync();
        }
    }
}
