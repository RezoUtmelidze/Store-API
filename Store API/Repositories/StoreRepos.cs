using Store_API.Data;

namespace Store_API.Repositories
{
    public class StoreRepos : StoreAPIRepos<Store>, IStoreRepos
    {
        private readonly StoreWADbContext _dbContext;
        public StoreRepos(StoreWADbContext dbContext) : base(dbContext)
        {
            _dbContext = dbContext;
        }
    }
}
