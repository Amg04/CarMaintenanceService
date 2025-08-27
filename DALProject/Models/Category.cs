using DALProject.Models.BaseClasses;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace DALProject.Models
{
    public class Category : IAllowedEntity
    {
        public int Id { get; set; }
        [Display(Name = "Category Name")]
        public required string Name { get; set; }
        [ValidateNever]
        [JsonIgnore]
        public ICollection<Technician> Technicians { get; set; } = new HashSet<Technician>();
        [ValidateNever]
        [JsonIgnore]
        public ICollection<Service> Services { get; set; } = new HashSet<Service>();
    }
}
