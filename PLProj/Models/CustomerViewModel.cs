using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace DALProject.Models
{
    public class CustomerViewModel
    {
        public string Id { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        [EmailAddress]
        public string Email { get; set; }
        [Required]
        public string City { get; set; }
        public string? Street { get; set; }
        [Required]
        [Display(Name = "Phone Number")]
        public string PhoneNumber { get; set; }
        [ValidateNever]
        public  ICollection<Car> Cars { get; set; } = new HashSet<Car>();
        public string? Role { get; set; }     
        public int NumberOfCars { get; set; }
    
        #region Mapping

        public static explicit operator CustomerViewModel(AppUser model)
        {
            return new CustomerViewModel
            {
                Id = model.Id,
                Name = model?.Name,
                Email = model?.Email,
                City = model?.City,
                Street = model?.Street,
                PhoneNumber = model?.PhoneNumber,
                Role = model?.Role,
                Cars = model.Cars,
            };
        }

        #endregion

    }
}
