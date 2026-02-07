using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Store_API.Data.Config
{
    public class CategoryConfig : IEntityTypeConfiguration<Category>
    {
        public void Configure(EntityTypeBuilder<Category> builder)
        {
            builder.ToTable("Categories");
            builder.HasKey(c => c.ID);
            builder.Property(c => c.ID).UseIdentityColumn();
            builder.Property(c => c.Name).IsRequired().HasMaxLength(100);
        }
    }
}
