using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Store_API.Data.Config
{
    public class ProductConfig : IEntityTypeConfiguration<Product>
    {
        public void Configure(EntityTypeBuilder<Product> builder)
        {
            builder.ToTable("Products");
            builder.HasKey(p => p.ID);
            builder.Property(p => p.ID).UseIdentityColumn();
            builder.Property(p => p.Name).IsRequired().HasMaxLength(70);
            builder.Property(p => p.Description).HasMaxLength(200);
            builder.Property(p => p.Price).IsRequired().HasPrecision(18, 2);
            builder.Property(p => p.ManufacturingDate).IsRequired();
            builder.Property(p => p.ExpirationDate).IsRequired();
            builder.HasMany(p => p.Categories);
        }
    }
}
