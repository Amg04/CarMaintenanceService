using DALProject.Models;
using Microsoft.EntityFrameworkCore;

namespace DALProject.Configrations
{
    public class ColorConfiguration : IEntityTypeConfiguration<Color>
    {
        public void Configure(Microsoft.EntityFrameworkCore.Metadata.Builders.EntityTypeBuilder<Color> builder)
        {
            builder.HasKey(e => e.Id);

            builder.Property(e => e.Name)
                .IsRequired()
                .HasMaxLength(100); 
        }
    }
}
