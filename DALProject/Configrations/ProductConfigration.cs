using DALProject.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DALProject.Configrations
{
    public class ProductConfiguration : IEntityTypeConfiguration<Product>
    {
        public void Configure(EntityTypeBuilder<Product> builder)
        {
            builder.HasKey(c => c.Id);

            builder.Property(p => p.Name)
                .IsRequired()
                .HasMaxLength(200);

            builder.Property(s => s.Description)
                     .IsRequired(false)
                     .HasMaxLength(2000);

            builder.Property(s => s.ImgPath)
              .IsRequired(false)
              .HasMaxLength(500)
              .HasDefaultValue("imgs/d54d7f37-bd9b-41cb-9086-e43c2f0c8fe9.jpg");

            builder.HasOne(e => e.ProductCategory)
              .WithMany()
              .HasForeignKey(e => e.ProdCatIegoryd)
              .OnDelete(DeleteBehavior.Cascade);
        }
    }
}

