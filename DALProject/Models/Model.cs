using DALProject.Models.BaseClasses;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.Collections.Generic;

namespace DALProject.Models
{
    public class Model : IAllowedEntity
    {
        public int Id { get; set; }
        public required string Name { get; set; }
        
        public int BrandId { get; set; }
        [ValidateNever]
        public  Brand Brand { get; set; } = null!;
        [ValidateNever]
        public  ICollection<Car> Cars { get; set; } = new HashSet<Car>();
        [ValidateNever]
        public ICollection<ModelPart> ModelParts { get; set; } = new HashSet<ModelPart>();
    }
}
