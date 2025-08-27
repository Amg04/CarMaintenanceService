using DALProject.Models.BaseClasses;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System;

namespace DALProject.Models
{
    public class OrderHeader :  IAllowedEntity
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        [ValidateNever]
        public AppUser AppUser { get; set; } = null!;
        public DateTime OrderDate { get; set; }
        public decimal TotalPrice { get; set; }
        public string? OrderStatus { get; set; }
        public string? PaymentStatus { get; set; }
        public string? TrackingNumber { get; set; }
        public DateTime? PaymentDate { get; set; }
        public DateTime? ShippingDate { get; set; }
        public string? DriverId { get; set; }
        [ValidateNever]
        public Driver? Driver { get; set; }
        //Stripe
        public string? SessionId { get; set; }
        public string? PaymentIntentId { get; set; }
        // User
        public  string Name { get; set; }
        public  string City { get; set; }
        public  string? Street { get; set; }
        public  string PhoneNumber { get; set; }
    }
}
