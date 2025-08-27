using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;

namespace DALProject.Models
{
    public class ModelViewModel 
    {
        public int Id { get; set; }
        [Required]
        [Display(Name = "Model Name")]
        public string Name { get; set; }
        [Required]
        [Display(Name = "Brand")]
        public int BrandId { get; set; }
        [ValidateNever]
        public  Brand Brand { get; set; }
        
        #region Mapping

        public static explicit operator ModelViewModel(Model model)
        {
            return new ModelViewModel
            {
                Id = model.Id,
                Name = model.Name,
                BrandId = model.BrandId,
                Brand = model.Brand,
            };
        }

        public static explicit operator Model(ModelViewModel viewModel)
        {
            return new Model
            {
                Id = viewModel.Id,
                Name = viewModel.Name,
                BrandId = viewModel.BrandId,
            };
        }

        #endregion
    }
}
