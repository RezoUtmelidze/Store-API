using Store_API.Data;

namespace Store_API.Repositories
{
    public interface IProductRepos : IStoreAPIRepos<Product>
    {
        Task<List<Product>> GetAllProductsWithCategoriesAsync();
        Task<List<Product>> GetProductsAsync(Func<Product, bool> filter);
    }
}
