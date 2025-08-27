using DALProject.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DALProject.Configrations
{
    public class ServiceConfigration : IEntityTypeConfiguration<Service>
    {
        public void Configure(EntityTypeBuilder<Service> builder)
        {
            builder.HasKey(c => c.Id);

            builder.Property(s => s.Name)
                .HasMaxLength(200)
                .IsRequired();

            builder.Property(s => s.Description)
               .IsRequired(false)
               .HasMaxLength(2000);

            builder.Property(p => p.RecommendedKilometres)
                  .HasDefaultValue(0);

            builder.Property(s => s.ImgPath)
               .IsRequired(false)
               .HasMaxLength(500)
               .HasDefaultValue("imgs/service-1.jpg");

            builder.HasOne(e => e.Category)
               .WithMany(t => t.Services)
               .HasForeignKey(e => e.CategoryId)
               .OnDelete(DeleteBehavior.NoAction);
        }
    }
}
