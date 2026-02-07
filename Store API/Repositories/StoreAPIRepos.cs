using Microsoft.EntityFrameworkCore;
using Store_API.Data;
using System.Linq.Expressions;

namespace Store_API.Repositories
{
    public class StoreAPIRepos<T> : IStoreAPIRepos<T> where T : class
    {
        private readonly StoreWADbContext _dbContext;
        private DbSet<T> _dbSet;
        public StoreAPIRepos(StoreWADbContext dbContext)
        {
            _dbContext = dbContext;
            _dbSet = dbContext.Set<T>();
        }

        public async Task<T> CreateAsync(T dbRecord)
        {
            _dbSet.Add(dbRecord);
            await _dbContext.SaveChangesAsync();
            return dbRecord;
        }

        public async Task<bool> DeleteAsync(T dbRecord)
        {
            _dbSet.Remove(dbRecord);
            await _dbContext.SaveChangesAsync();
            return true;
        }

        public async Task<List<T>> GetAllAsync()
        {
            return await _dbSet.ToListAsync();
        }

        public async Task<T> GetAsync(Expression<Func<T, bool>> filter)
        {
            return await _dbSet.AsNoTracking().FirstOrDefaultAsync(filter);
        }

        public async Task<T> UpdateAsync(T dbRecord)
        {
            _dbSet.Update(dbRecord);
            await _dbContext.SaveChangesAsync();
            return dbRecord;
        }
    }
}
