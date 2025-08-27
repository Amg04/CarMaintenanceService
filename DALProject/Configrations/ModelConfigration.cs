using DALProject.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DALProject.Configrations
{
    internal class ModelConfigration : IEntityTypeConfiguration<Model>
    {
        public void Configure(EntityTypeBuilder<Model> builder)
        {
            builder.HasKey(c => c.Id);

            builder.Property(m => m.Name)
           .IsRequired()
            .HasMaxLength(200);
        }
    }
}
