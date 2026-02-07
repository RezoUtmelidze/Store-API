using Microsoft.EntityFrameworkCore;
using Store_API.Data;

namespace Store_API.Repositories
{
    public class EmployeeRepos : StoreAPIRepos<Employee>, IEmployeeRepos
    {
        private readonly StoreWADbContext _dbContext;
        public EmployeeRepos(StoreWADbContext dbContext) : base(dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<List<Employee>> GetEmployeesAsync(Func<Employee, bool> filter)
        {
            return await _dbContext.Employees.Where(e => filter(e)).ToListAsync();
        }
    }
}
