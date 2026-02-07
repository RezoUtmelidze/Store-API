using Microsoft.AspNetCore.DataProtection;

namespace Store_API.Data
{
    public class Store
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public bool Open { get; set; }
        public ICollection<Employee> Employees { get; set; }
    }
}
