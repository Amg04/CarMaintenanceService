using DALProject.Models;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace PLProj.Models
{
    public class ProductViewModel
    {
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
        public string? Description { get; set; }
        [Display(Name = "Image")]
        public string? ImgPath { get; set; }
        [Required]
        public decimal Price { get; set; }
        [Display(Name = "Product Category")]
        public int ProdCategoryId { get; set; }
        [ValidateNever]
        public ProductCategory ProductCategory { get; set; }
        // لإدارة العلاقة many-to-many
        [Display(Name = "Associated Models")]
        [ValidateNever]
        public List<int> SelectedModelIds { get; set; } = new();

        #region Mapping
        public static explicit operator ProductViewModel(Product model)
        {
            return new ProductViewModel
            {
                Id = model.Id,
                Name = model.Name,
                Price = model.Price,
                ImgPath = model.ImgPath,
                Description = model.Description,
                ProdCategoryId = model.ProdCatIegoryd,
                ProductCategory = model.ProductCategory,
                SelectedModelIds = model.ModelParts?.Select(mp => mp.ModelId).ToList() ?? new List<int>()
            };
        }

        public static explicit operator Product(ProductViewModel viewModel)
        {
            return new Product
            {
                Id = viewModel.Id,
                Name = viewModel.Name,
                Price = viewModel.Price,
                ImgPath = viewModel.ImgPath,
                ProdCatIegoryd = viewModel.ProdCategoryId,
                Description = viewModel.Description,
            };
        }
        #endregion
    }
}
