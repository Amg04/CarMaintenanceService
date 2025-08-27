using DALProject.Models.BaseClasses;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace DALProject.Models
{
    public class AppUser : IdentityUser,IAllowedEntity
    {
        public  string Name { get; set; }
        public  string City { get; set; }
        public string? Street { get; set; }
        public  string Role { get; set; }

        [ValidateNever]
        public ICollection<Car> Cars { get; set; } = new HashSet<Car>();
        [ValidateNever]
        [JsonIgnore]
        public Driver? Driver { get; set; }
    }
}