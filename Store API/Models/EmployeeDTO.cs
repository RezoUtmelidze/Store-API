using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Data.SqlTypes;

namespace Store_API.Models
{
    public class EmployeeDTO
    {
        public int ID { get; set; }
        [Required, Range(3,255)]
        public string Name { get; set; }
        [Required, Range(3,255)]
        public string Lastname { get; set; }
        [Required]
        public int Age { get; set; }
        [Required]
        public decimal Salary { get; set; }
        public DateTime LastPaid { get; set; }
        [Required]
        public int PayRollCycle { get; set; } //In weeks
        [Required]
        public string Position { get; set; }
        [Required]
        public int StoreID { get; set; }
        public bool IsTimeToPay() => (DateTime.Now - LastPaid).TotalDays >= (PayRollCycle * 7);
    }
}
