using DALProject.Models.BaseClasses;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace DALProject.Models
{
	public class KiloMetres : IAllowedEntity
	{
        public long kiloMetre { get; set; }
		public int CarId { get; set; }
        [ValidateNever]
        public  Car Car { get; set; } = null!;
	}
}
