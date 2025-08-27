using DALProject.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DALProject.Configrations
{
    internal class DriverConfigurations : IEntityTypeConfiguration<Driver>
    {
        public void Configure(EntityTypeBuilder<Driver> builder)
        {
            builder.HasKey(c => c.Id);

            builder.Property(e => e.License)
                .IsRequired()
                .HasMaxLength(100);

            builder.HasOne(e => e.User)
                .WithOne(u => u.Driver)
                .HasForeignKey<Driver>(e => e.Id);
        }
    }
}
