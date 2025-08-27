using DALProject.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace DALProject.Configrations
{
    internal class TicketConfigration : IEntityTypeConfiguration<Ticket>
    {
        public void Configure(EntityTypeBuilder<Ticket> builder)
        {
            builder.HasKey(c => c.Id);

            builder.Property(e => e.CreatedAt)
                .HasDefaultValueSql("GETDATE()");

            builder.Property(e => e.Location)
               .IsRequired()
               .HasMaxLength(200);

            builder.Property(e => e.stateType)
                .HasConversion(new EnumToStringConverter<StateType>());

            builder.Property(s => s.FinalReport)
            .IsRequired(false)
            .HasMaxLength(2000);

            builder.Property(e => e.Feedback)
               .IsRequired(false)
                .HasMaxLength(1500);

            builder.HasOne(e => e.Service)
                .WithMany(t => t.Tickets)
                .HasForeignKey(e => e.ServiceId)
                .OnDelete(DeleteBehavior.NoAction);

            builder.HasOne(e => e.Car)
                .WithMany(t => t.Tickets)
                .HasForeignKey(e => e.CarId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
