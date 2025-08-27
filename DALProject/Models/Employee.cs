using DALProject.Models.BaseClasses;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace DALProject.Models
{
    abstract public class Employee : IAllowedEntity
    {
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public required string Id { get; set; }
        [ForeignKey(nameof(Id))]
        [ValidateNever]
        public AppUser User { get; set; } = null!;
		public bool Availability { get; set; }
        public DateOnly BirthDate  { get; set; }
    }
}
