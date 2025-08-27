using System.ComponentModel.DataAnnotations;

namespace PLProj.Models
{
    public class KilometreViewModel
    {
        [Required]
        public int CarId { get; set; }
        [Display(Name = "Current Kilometre")]
        [Required(ErrorMessage = "Please enter the current kilometre.")]
        [Range(1, int.MaxValue, ErrorMessage = "Value must be a Greater than 0.")]
        public long kiloMetre { get; set; }
        public string? PlateNumber { get; set; } // For display only
    }
}
