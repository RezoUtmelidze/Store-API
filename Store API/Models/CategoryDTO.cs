using System.ComponentModel.DataAnnotations;

namespace Store_API.Models
{
    public class CategoryDTO
    {
        public int ID { get; set; }
        [Required,MaxLength (100)]
        public string Name { get; set; }
    }
}
