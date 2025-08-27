using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System;
using System.Collections.Generic;

namespace DALProject.Models
{
    public class  Driver : Employee
    {
        public required string License  { get; set; }
        public DateOnly LicenseDate { get; set; }
        public DateOnly LicenseExpDate { get; set; }
        [ValidateNever]
        public  ICollection<Appointment> Appointments { get; set; } = new HashSet<Appointment>();
        [ValidateNever]
        public  ICollection<OrderHeader> OrderHeaders { get; set; } = new HashSet<OrderHeader>();
    }
   

}
