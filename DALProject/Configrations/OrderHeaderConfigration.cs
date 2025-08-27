using DALProject.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DALProject.Configrations
{
    internal class OrderHeaderConfigration : IEntityTypeConfiguration<OrderHeader>
    {
        public void Configure(EntityTypeBuilder<OrderHeader> builder)
        {
            builder.HasKey(e => e.Id);

            builder.Property(e => e.UserId)
                .IsRequired();

            builder.HasOne(e => e.AppUser)
                .WithMany()
                .HasForeignKey(e => e.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Property(e => e.OrderStatus)
                .HasMaxLength(200)
                .IsRequired(false);

            builder.Property(e => e.PaymentStatus)
                .HasMaxLength(200)
                .IsRequired(false);

            builder.Property(e => e.TrackingNumber)
                .HasMaxLength(500)
                .IsRequired(false);

            builder.HasOne(e => e.Driver)
              .WithMany(or => or.OrderHeaders)
              .HasForeignKey(e => e.DriverId)
              .OnDelete(DeleteBehavior.NoAction);

            builder.Property(e => e.SessionId)
               .HasMaxLength(500)
               .IsRequired(false);

            builder.Property(e => e.PaymentIntentId)
               .HasMaxLength(500)
               .IsRequired(false);

            builder.Property(e => e.Name)
              .HasMaxLength(200)
              .IsRequired(true);

            builder.Property(e => e.City)
              .HasMaxLength(500)
              .IsRequired(true);

            builder.Property(e => e.Street)
              .HasMaxLength(300)
              .IsRequired(false);

            builder.Property(e => e.PhoneNumber)
              .HasMaxLength(50)
              .IsRequired(true);
        }
    }
}
