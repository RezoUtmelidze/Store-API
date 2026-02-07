using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Store_API.Data.Config
{
    public class EmployeeConfig : IEntityTypeConfiguration<Employee>
    {
        public void Configure(EntityTypeBuilder<Employee> builder)
        {
            builder.ToTable("Employees");
            builder.HasKey(e => e.ID);
            builder.Property(e => e.ID).UseIdentityColumn();
            builder.Property(e => e.Name).IsRequired().HasMaxLength(50);
            builder.Property(e => e.Lastname).IsRequired().HasMaxLength(70);
            builder.Property(e => e.Age).IsRequired();
            builder.Property(e => e.Salary).HasPrecision(18,2);
            builder.Property(e => e.LastPaid);
            builder.Property(e => e.PayRollCycle).HasDefaultValue(4);
            builder.Property(e => e.Position).IsRequired();
            builder.HasOne(e => e.Store).WithMany(s => s.Employees).HasForeignKey(e => e.StoreID);
        }
    }
}
