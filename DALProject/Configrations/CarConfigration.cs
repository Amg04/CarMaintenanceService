using DALProject.Models;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;

namespace DALProject.Configrations
{
    internal class CarConfigration : IEntityTypeConfiguration<Car>
    {
        public void Configure(EntityTypeBuilder<Car> builder)
        {
            builder.HasKey(e => e.Id);

            builder.Property(c => c.PlateNumber)
               .IsRequired()
               .HasMaxLength(200);

            builder.Property(c => c.Description)
                .HasMaxLength(2000);

            builder.HasOne(e => e.AppUser)
                .WithMany(s => s.Cars)
                .HasForeignKey(e => e.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(e => e.Color)
                .WithMany(s => s.Cars)
                .HasForeignKey(e => e.ColorId)
                .OnDelete(DeleteBehavior.NoAction);

            builder.HasOne(e => e.Model)
               .WithMany(s => s.Cars)
               .HasForeignKey(e => e.ModelId)
               .OnDelete(DeleteBehavior.NoAction);
        }
    }

}