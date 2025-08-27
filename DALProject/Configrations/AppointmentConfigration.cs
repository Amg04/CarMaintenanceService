using DALProject.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DALProject.Configrations
{
    internal class AppointmentConfigration : IEntityTypeConfiguration<Appointment>
    {
        public void Configure(EntityTypeBuilder<Appointment> builder)
        {
            builder.HasKey(e => e.Id);

            builder.Property(e => e.PartialReport)
                .HasMaxLength(2000);

            builder.Property(e => e.StartDateTime)
                .HasColumnType("datetime2");

            builder.Property(e => e.EndDateTime)
                .HasColumnType("datetime2");

            builder.Property(e => e.TechnicianId)
              .IsRequired();

            builder.HasOne(e => e.Technician)
                .WithMany(s => s.Appointments)
                .HasForeignKey(e => e.TechnicianId)
                .OnDelete(DeleteBehavior.NoAction);

            builder.HasOne(e => e.Driver)
               .WithMany(s => s.Appointments)
               .HasForeignKey(e => e.DriverId)
               .OnDelete(DeleteBehavior.NoAction);

            builder.Property(e => e.TicketId)
                .IsRequired();

            builder.HasOne(e => e.Ticket)
                .WithMany(t => t.Appointments)
                .HasForeignKey(e => e.TicketId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
