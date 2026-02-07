using Store_API.Data;

namespace Store_API.Repositories
{
    public class CategoryRepos : StoreAPIRepos<Category>, ICategoryRepos
    {
        private readonly StoreWADbContext _dbContext;
        public CategoryRepos(StoreWADbContext dbContext) : base(dbContext)
        {
            _dbContext = dbContext;
        }
    }
}
