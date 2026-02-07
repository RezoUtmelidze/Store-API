using Store_API.Data;

namespace Store_API.Repositories
{
    public interface IEmployeeRepos : IStoreAPIRepos<Employee>
    {
        Task<List<Employee>> GetEmployeesAsync(Func<Employee, bool> filter);
    }
}
