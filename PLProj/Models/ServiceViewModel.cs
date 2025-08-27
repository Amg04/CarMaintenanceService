using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;

namespace DALProject.Models
{
    public class ServiceViewModel 
    {
        public int Id { get; set; }
        [Display(Name = "Category")]
        public int CategoryId { get; set; }
        [Required]
        public string Name { get; set; }
        [Display(Name = "Image")]
        public string? ImgPath { get; set; }
        [DataType(DataType.Currency)]
        [Required]
        public decimal Price{ get; set; }
        public string? Description{ get; set; }
        [Display(Name = "Kilometres to Change Part")]
        public int? RecommendedKilometres { get; set; }
        [ValidateNever]
        public  Category Category { get; set; } 
        #region Mapping
        public static explicit operator ServiceViewModel(Service model)
        {
            return new ServiceViewModel
            {
                Id = model.Id,
                CategoryId = model.CategoryId,
                Name = model.Name,
                ImgPath = model.ImgPath,
                Price = model.Price,
                Description = model.Description,
                RecommendedKilometres = model.RecommendedKilometres,
                Category = model.Category
            };
        }

        public static explicit operator Service(ServiceViewModel viewModel)
        {
            return new Service
            {
                Id = viewModel.Id,
                CategoryId = viewModel.CategoryId,
                Name = viewModel.Name,
                ImgPath = viewModel.ImgPath,
                Price = viewModel.Price,
                Description = viewModel.Description,
                RecommendedKilometres = viewModel.RecommendedKilometres,
            };
        }
        #endregion
    }
}







