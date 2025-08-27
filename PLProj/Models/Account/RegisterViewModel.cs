using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
namespace PLProj.Models.Account
{
    public class RegisterViewModel
    {
        [Required]
        public string Name { get; set; }
        [Required]
        [EmailAddress]
        public string Email { get; set; }
        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", 
            MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm password")]
        [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }
        [Required]
        public string City { get; set; }
        public string? Street { get; set; }
        [Required]
        [Display(Name = "Phone Number")]
        public string PhoneNumber { get; set; }
        public string? Role { get; set; }
        [ValidateNever]
        public IEnumerable<SelectListItem>? RoleList { get; set; }
        
        // employee
        public bool Availability { get; set; }
        public DateOnly? BirthDate { get; set; }
        public string? AppUserId { get; set; }
        
        // if Tech
        [ValidateNever]
        public IEnumerable<SelectListItem>? CategoryList { get; set; }
        [Display(Name = "Category Name")]
        public int? CategoryId { get; set; }
        [Display(Name="Image")]
        public string? Img { get; set; }

        // if Driver
        public string? License { get; set; }
        public DateOnly? LicenseDate { get; set; }
        public DateOnly? LicenseExpDate { get; set; }
        

    }
}
