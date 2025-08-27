using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace PLProj.Models
{
    public class EditAppointmentViewModel
    {
        public int id { get; set; }
        [Display(Name = "Start Date & Time")]
        public DateTime? StartDateTime { get; set; }
        [Display(Name = "End Date & Time")]
        public DateTime? EndDateTime { get; set; }
        public string? PartialReport { get; set; }
        [Display(Name = "Ticket")]
        public int TicketId { get; set; }
        [Display(Name = "Driver")]
        public string? DriverId { get; set; }
        [Required]
        [Display(Name ="Technician")]
        public string TechId { get; set; }
        [ValidateNever]
        public List<SelectListItem> Technician { get; set; }
        [ValidateNever]
        public List<SelectListItem> Driver { get; set; }      
    }
}



