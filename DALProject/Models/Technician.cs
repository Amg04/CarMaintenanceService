using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace DALProject.Models
{
    public class Technician : Employee
    {
        public int CategoryId { get; set; }
        [ValidateNever]
        public  Category Category { get; set; } = null!;
        [ValidateNever]
        public string? ImgPath { get; set; }
        [ValidateNever]
        public  ICollection<Appointment> Appointments { get; set; } = new HashSet<Appointment>();      
    }
}
