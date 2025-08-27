using DALProject.Models.BaseClasses;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace DALProject.Models
{
   
    public class Service : IAllowedEntity
    {
        public int Id { get; set; }
        public required string Name { get; set; }
        public decimal Price{ get; set; }
        [ValidateNever]
        public string? ImgPath { get; set; }
        public int? RecommendedKilometres { get; set; }
        public string? Description{ get; set; }
        public int CategoryId { get; set; }
        [ValidateNever]
        public  Category Category { get; set; } = null!;
        [ValidateNever]
        [JsonIgnore]
        public ICollection<Ticket> Tickets { get; set; } = new HashSet<Ticket>();
    }


}

