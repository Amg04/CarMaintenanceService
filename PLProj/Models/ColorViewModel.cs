using System.ComponentModel.DataAnnotations;

namespace DALProject.Models
{
    public class ColorViewModel
    {
        public int Id { get; set; }
        [Display(Name = "Color")]
        [StringLength(50, ErrorMessage = "The Color Name must not exceed 50 characters.")]
        public string Name { get; set; }

        #region Mapping

        public static explicit operator ColorViewModel(Color model)
        {
            return new ColorViewModel
            {
                Id = model.Id,
                Name = model.Name,
            };
        }

        public static explicit operator Color(ColorViewModel ViewModel)
        {
            return new Color
            {
                Id = ViewModel.Id,
                Name = ViewModel.Name,
            };
        }

        #endregion

    }
}
