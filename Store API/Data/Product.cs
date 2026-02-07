using System.Data.SqlTypes;

namespace Store_API.Data
{
    public class Product
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public DateTime ManufacturingDate { get; set; }
        public DateTime ExpirationDate { get; set; }
        public virtual ICollection<Category> Categories { get; set; }
        public bool IsSpoiled() => ExpirationDate > DateTime.Now;
    }
}
