using Store_API.Data;
using System.Linq.Expressions;

namespace Store_API.Repositories
{
    public interface IStoreAPIRepos<T>
    {
        Task<List<T>> GetAllAsync();
        Task<T> GetAsync(Expression<Func<T, bool>> filter);
        Task<T> CreateAsync(T dbRecord);
        Task<T> UpdateAsync(T dbRecord);
        Task<bool> DeleteAsync(T dbRecord);
    }
}
