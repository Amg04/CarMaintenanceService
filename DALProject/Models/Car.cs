using DALProject.Models.BaseClasses;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace DALProject.Models
{
    public class Car : IAllowedEntity
    {
        public int Id { get; set; }
        public required string PlateNumber { get; set; }
        public int Year { get; set; }
        public string? Description { get; set; }
        [ValidateNever]
        [JsonIgnore]
        public  ICollection<Ticket> Tickets { get; set; } = new HashSet<Ticket>();
        [ValidateNever]
        public  ICollection<KiloMetres> KiloMetres { get; set; } = new HashSet<KiloMetres>();
        public required string UserId { get; set; }
        [ValidateNever]
        [JsonIgnore]
        public AppUser AppUser { get; set; } = null!;
        public int ColorId { get; set; }
        [ValidateNever]
        public  Color Color { get; set; } = null!;
        public int ModelId { get; set; }
        [ValidateNever]
        public  Model Model { get; set; } = null!;
    }
}
