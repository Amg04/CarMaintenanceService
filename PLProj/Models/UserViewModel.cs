using DALProject.Models;
using System;

namespace PLProj.Models
{
    public class UserViewModel
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string PhoneNumber { get; set; }
        public string Email { get; set; }
        public string Role { get; set; }
        public bool IsLocked { get; set; }


        #region Maping
        public static explicit operator UserViewModel(AppUser model)
        {
            return new UserViewModel
            {
                Id = model.Id,
                Name = model?.Name,
                Email = model?.Email,
                PhoneNumber = model?.PhoneNumber,
                Role = model?.Role,
                IsLocked = model.LockoutEnd != null && model.LockoutEnd > DateTime.Now
            };
        }
        #endregion
    }
}
