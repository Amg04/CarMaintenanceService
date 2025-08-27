using DALProject.Models.BaseClasses;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.Collections.Generic;

namespace DALProject.Models
{
    public class Color : IAllowedEntity
    {
        public int Id { get; set; }
        public required string Name { get; set; }
        [ValidateNever]
        public  ICollection<Car> Cars { get; set; } = new HashSet<Car>();

    }
}
