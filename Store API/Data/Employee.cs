using System.Data.SqlTypes;

namespace Store_API.Data
{
    public class Employee
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public string Lastname { get; set; }
        public int Age { get; set; }
        public decimal Salary { get; set; }
        public DateTime LastPaid { get; set; }
        public int PayRollCycle { get; set; } //In weeks
        public string Position { get; set; }
        public int StoreID { get; set; }
        public Store Store { get; set; }
        public bool IsTimeToPay() => (DateTime.Now - LastPaid).TotalDays >= (PayRollCycle * 7);
    }
}
