using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System;
using System.ComponentModel.DataAnnotations;
namespace DALProject.Models
{
    public class TechnicianViewModel
    {
        public string Id { get; set; }
        [Required]
        [Display(Name = "Technician Name")]
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
        [Display(Name = "Category Name")]
        [Required]
        public int CategoryId { get; set; }

        [Display(Name = "Image")]
        public string? Img { get; set; }
        public string? Role { get; set; }
        [ValidateNever]
        public virtual Category Category { get; set; }
        [ValidateNever]
        public AppUser User { get; set; }

        #region Mapping

        public static explicit operator TechnicianViewModel(Technician model)
        {
            return new TechnicianViewModel
            {
                Id = model.Id,
                Name = model.User?.Name,
                Email = model.User?.Email,
                City = model.User?.City,
                Street = model.User?.Street,
                PhoneNumber = model.User?.PhoneNumber,
                Role = model.User.Role,
                Availability = model.Availability,
                BirthDate = model.BirthDate,
                Category = model.Category,
                CategoryId = model.CategoryId,
                Img = model.ImgPath,
                User = model.User,
            };
        }


        #endregion
    }
}
