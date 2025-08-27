using DALProject.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DALProject.Configrations
{
    internal class AppUserConfigration : IEntityTypeConfiguration<AppUser>
    {
        public void Configure(EntityTypeBuilder<AppUser> builder)
        {

            builder.Property(e => e.Name)
                .IsRequired()
                .HasMaxLength(200);

            builder.Property(e => e.City)
                .IsRequired()
                .HasMaxLength(200);

            builder.Property(e => e.Street)
               .HasMaxLength(200);

            builder.Property(e => e.Role)
                .IsRequired()
                .HasMaxLength(200);
        }
    }
}
