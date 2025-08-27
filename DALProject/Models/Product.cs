using DALProject.Models.BaseClasses;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.Collections.Generic;

namespace DALProject.Models
{
    public class Product : IAllowedEntity
    {
        public int Id { get; set; }
        public required string Name { get; set; }
        public string? Description { get; set; }
        [ValidateNever]
        public string? ImgPath { get; set; }
        public decimal Price { get; set; }
        public int ProdCatIegoryd { get; set; }
        [ValidateNever]
        public  ProductCategory ProductCategory { get; set; } = null!;
        public ICollection<ModelPart> ModelParts { get; set; } = new HashSet<ModelPart>();
    }
}
