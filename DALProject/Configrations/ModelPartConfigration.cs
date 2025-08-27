using DALProject.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DALProject.Configrations
{
    internal class ModelPartConfigration : IEntityTypeConfiguration<ModelPart>
    {
        public void Configure(EntityTypeBuilder<ModelPart> builder)
        {
           builder.HasKey(mp => new { mp.ModelId, mp.ProductId });

            // العلاقة مع Model
            builder.HasOne(mp => mp.Model)
                   .WithMany(m => m.ModelParts)
                   .HasForeignKey(mp => mp.ModelId);

            // العلاقة مع Part
            builder.HasOne(mp => mp.Product)
                   .WithMany(p => p.ModelParts)
                   .HasForeignKey(mp => mp.ProductId);
        }
    }
}
