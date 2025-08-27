using DALProject.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DALProject.Configrations
{
    internal class TechnicalConfigurations : IEntityTypeConfiguration<Technician>
    {
        public void Configure(EntityTypeBuilder<Technician> builder)
        {
            builder.HasKey(c => c.Id);

            builder.HasOne(e => e.Category)
              .WithMany(s => s.Technicians)
              .HasForeignKey(e => e.CategoryId)
              .OnDelete(DeleteBehavior.NoAction);

            builder.Property(s => s.ImgPath)
               .IsRequired(false)
               .HasMaxLength(500)
               .HasDefaultValue("imgs/client.png");

            builder.HasOne(e=>e.User)
                .WithOne()
                .HasForeignKey<Technician>(e => e.Id);

        }
    }
}

