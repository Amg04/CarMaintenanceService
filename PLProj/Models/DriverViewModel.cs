using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DALProject.Models
{
    public class  DriverViewModel 
    {

        public string Id { get; set; }
        [Required]
        [Display(Name = "Driver Name")]
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
        public bool Availability { get; set; }
        public DateOnly BirthDate { get; set; } 
        [Required]
        public string License  { get; set; }
        [Display(Name = "License Date")]
        [Required]
        public DateOnly LicenseDate { get; set; }
        [Required]
        [Display(Name = "License Expiry Date")]
        public DateOnly LicenseExpDate { get; set; }
        public string? Role { get; set; }
        [ValidateNever]
        public AppUser User { get; set; }



        #region Mapping

        public static explicit operator DriverViewModel(Driver model)
        {
            return new DriverViewModel
            {
                Id = model.Id,
                Name = model.User?.Name,
                Email = model.User?.Email,
                City = model.User?.City,
                Street = model.User?.Street,
                PhoneNumber = model.User?.PhoneNumber,
                Role = model.User?.Role,
                Availability = model.Availability,
                BirthDate = model.BirthDate,
                License = model.License,
                LicenseDate = model.LicenseDate,
                LicenseExpDate = model.LicenseExpDate,
                User = model.User,
            };
        }

        #endregion

    }    
}
