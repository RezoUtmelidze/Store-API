using Store_API.Data;
using System.ComponentModel.DataAnnotations;
using System.Data.SqlTypes;

namespace Store_API.Models
{
    public class ProductDTO
    {
        public int ID { get; set; }
        [Required, Range(3, 255)]
        public string Name { get; set; }
        [Required, MaxLength(1000)]
        public string Description { get; set; }
        [Required]
        public decimal Price { get; set; }
        [Required]
        public DateTime ManufacturingDate { get; set; }
        [Required]
        public DateTime ExpirationDate { get; set; }
        [Required]
        public virtual ICollection<int> CategoryIDs { get; set; }
        public bool IsSpoiled() => ExpirationDate > DateTime.Now;
    }
}
