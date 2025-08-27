using DALProject.Models;
using System;
using System.ComponentModel.DataAnnotations;

namespace PLProj.Models
{
    public class AppointmentDetailViewModel
    {
        [Display(Name = "Technician Name")]
        public string? TechnicianName { get; set; }
        [Display(Name = "Technician PhoneNumber")]
        public string? TechPhone { get; set; }
        [Display(Name = "Driver Name")]
        public string? DriverName { get; set; }
        [Display(Name = "Driver PhoneNumber")]
        public string? DriverPhone { get; set; }
        [Display(Name = "Ticket Id")]
        public int TicketId { get; set; }
        [Display(Name = "Start Date & Time")]
        public DateTime? StartDataTime { get; set; }
        [Display(Name = "End Date & Time")]
        public DateTime? EndDateTime { get; set; }
        public string? PartialReport { get; set; }
        public Technician Technician { get; set; }
        public Driver? Driver { get; set; }

        #region Mapping
        public static explicit operator AppointmentDetailViewModel(Appointment model)
        {
            var viewmodel = new AppointmentDetailViewModel()
            {
                TechnicianName = model.Technician?.User?.Name,
                DriverName = model.Driver?.User?.Name,
                DriverPhone = model.Driver?.User?.PhoneNumber,
                TechPhone = model.Technician?.User?.PhoneNumber,
                TicketId = model.TicketId,
                StartDataTime = model.StartDateTime,
                EndDateTime = model.EndDateTime,
                PartialReport = model.PartialReport,
                Technician = model.Technician,
                Driver = model.Driver,
            };
            return viewmodel;
        }
        #endregion
    }
}
