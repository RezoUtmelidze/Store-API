using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Store_API.Data.Config
{
    public class StoreConfig : IEntityTypeConfiguration<Store>
    {
        public void Configure(EntityTypeBuilder<Store> builder)
        {
            builder.ToTable("Stores");
            builder.HasKey(s => s.ID);
            builder.Property(s => s.ID).UseIdentityColumn();
            builder.Property(s => s.Name).IsRequired().HasMaxLength(50);
            builder.Property(s => s.Description).IsRequired().HasMaxLength(300);
            builder.Property(s => s.Open).IsRequired().HasDefaultValue(false);
            builder.HasMany(s => s.Employees).WithOne(e => e.Store).HasForeignKey(e => e.StoreID);

            builder.HasData(new List<Store>() {
            new Store{ID = 1, Name = "Main Store", Description = "The primary store location.", Open = true},
            new Store{ID = 2, Name = "Second Store", Description = "Backup store location.", Open = false}
            }
        );
        }
    }
}
