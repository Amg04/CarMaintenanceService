using DALProject.Models;
using System;
using System.ComponentModel.DataAnnotations;

namespace PLProj.Models
{
    public class ProductCategoryVM
    {
        public int Id { get; set; }
        [Required]
        [Display(Name = "Category Name")]
        public  string Name { get; set; }
        public string? Description { get; set; }

        #region Mapping
        public static explicit operator ProductCategoryVM(ProductCategory model)
        {
            return new ProductCategoryVM
            {
                Id = model.Id,
                Name = model.Name,
                Description = model.Description,
            };
        }

        public static explicit operator ProductCategory(ProductCategoryVM viewModel)
        {
            return new ProductCategory
            {
                Id = viewModel.Id,
                Name = viewModel.Name,
                Description = viewModel.Description,
            };
        }
        #endregion
    }
}
