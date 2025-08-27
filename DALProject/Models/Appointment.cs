using DALProject.Models.BaseClasses;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System;
namespace DALProject.Models
{
    public class Appointment : IAllowedEntity
    {
        public int Id { get; set; }
        public string? PartialReport { get; set; } 
        public DateTime? StartDateTime { get; set; }
        public DateTime? EndDateTime { get; set; }
        public required string TechnicianId { get; set; }
        [ValidateNever]
        public Technician Technician { get; set; } = null!;
        public string? DriverId { get; set; }
        [ValidateNever]
        public  Driver? Driver { get; set; }
        public int TicketId { get; set; }
        [ValidateNever]
        public Ticket Ticket { get; set; } = null!;
    }
}
