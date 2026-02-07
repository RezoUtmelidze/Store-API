using Microsoft.AspNetCore.DataProtection;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Store_API.Models
{
    public class StoreDTO
    {
        public int ID { get; set; }
        [Required,Length(3,100)]
        public string Name { get; set; }
        [Required, MaxLength(1000)]
        public string Description { get; set; }
        [DefaultValue(true)]
        public bool Open { get; set; }
    }
}
