using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;

namespace DALProject.Models
{
	public class CarViewModel
	{
        public int Id { get; set; }
		[Display(Name = "Plate Number")]
		[Required]
		public string PlateNumber { get; set; }

        [Display(Name = "Model")]
        [Required(ErrorMessage = "Please select a model.")]
        public int ModelId { get; set; }
        
		[Display(Name = "Brand")]
        [Required(ErrorMessage = "Please select a brand.")]
        public int BrandId { get; set; }
		public string? UserId { get; set; }
		[Required]
		[Display(Name = "Color")]
		public int ColorId { get; set; }
		[Required]

		[Display(Name = "Year of Manufacture")]
		public int Year { get; set; }

		[Display(Name = "Description")]
		public string? Description { get; set; }
        [ValidateNever]
        public  Model Model { get; set; }
        [ValidateNever]
        public  Color Color { get; set; } 


		#region Mapping

		public static explicit operator CarViewModel(Car model)
		{
			var viewmodel = new CarViewModel()
			{
				Id = model.Id,
				ModelId = model.ModelId,
				UserId = model.UserId,
				ColorId = model.ColorId,
				Year = model.Year,
				Description = model.Description,
				Model = model.Model,
				Color = model.Color,
				PlateNumber = model.PlateNumber,

			};
			viewmodel.Model.Brand = model.Model.Brand;
			return viewmodel;
        }

        public static explicit operator Car(CarViewModel viewModel)
		{
			return new Car
			{
				Id = viewModel.Id,
				ModelId = viewModel.ModelId,
				UserId = viewModel.UserId,
				ColorId = viewModel.ColorId,
				Year = viewModel.Year,
				Description = viewModel.Description,
				PlateNumber= viewModel.PlateNumber,
			};
		}

		#endregion

	}
}
