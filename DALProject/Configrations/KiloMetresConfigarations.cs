using DALProject.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DALProject.Configrations
{
	internal class KiloMetresConfigarations : IEntityTypeConfiguration<KiloMetres>
	{
		public void Configure(EntityTypeBuilder<KiloMetres> builder)
		{
            builder.HasKey(e => new { e.CarId, e.kiloMetre });

            builder.HasOne(e => e.Car)
               .WithMany(t => t.KiloMetres)
               .HasForeignKey(e => e.CarId)
               .OnDelete(DeleteBehavior.Cascade);
        }
	}
}
