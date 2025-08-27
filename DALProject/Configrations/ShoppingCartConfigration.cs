using DALProject.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DALProject.Configrations
{
    internal class ShoppingCartConfigration : IEntityTypeConfiguration<ShoppingCart>
    {
        public void Configure(EntityTypeBuilder<ShoppingCart> builder)
        {
            builder.HasKey(e => e.Id);

            builder.HasOne(e => e.Product)
              .WithMany()
              .HasForeignKey(e => e.ProductId)
              .OnDelete(DeleteBehavior.NoAction);


            builder.Property(e => e.UserId)
             .IsRequired();

            builder.HasOne(e => e.User)
              .WithMany()
              .HasForeignKey(e => e.UserId)
              .OnDelete(DeleteBehavior.NoAction);

            builder.ToTable(t =>
            {
                t.HasCheckConstraint("CK_ShoppingCart_Count_Range", "[count] >= 1 AND [count] <= 100");
            });

        }
    }
   
}
