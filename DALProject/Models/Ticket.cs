using DALProject.Models.BaseClasses;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System;
using System.Collections.Generic;

namespace DALProject.Models
{
    public enum StateType
    {
        New = 1,
        Assigned = 2,
        Finished = 3
    }
    public class Ticket : IAllowedEntity
    {
        public int Id { get; set; }
        public long  CurrentKilometres { get; set; }    
        public DateTime? StartDateTime { get; set; }    
        public DateTime CreatedAt { get; set; }    
        public string Location { get; set; }    
        public  StateType stateType { get; set; }     
        public string? FinalReport { get; set; }
        public DateTime? EndDateTime { get; set; }
        public string? Feedback { get; set; }
        public int ServiceId { get; set; }
        [ValidateNever]
        public Service Service { get; set; } = null!;
        [ValidateNever]
        public  ICollection<Appointment> Appointments { get; set; } = new HashSet<Appointment>();
        public int CarId { get; set; }
        [ValidateNever]
        public  Car Car { get; set; } = null!;

         //✅ Stripe Payment Info
        public string? SessionId { get; set; }
        public string? PaymentIntentId { get; set; }
        public string? PaymentStatus { get; set; }
        public DateTime? PaymentDate { get; set; }
    }

}

