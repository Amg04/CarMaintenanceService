using DALProject.Models;
using System.ComponentModel.DataAnnotations;
using System;

namespace PLProj.Models
{
    public class AddAppointmentViewModel
    {
        [Display(Name = "Start Date & Time")]
        [Required]
        public DateTime StartDateTime { get; set; }
        [Required]
        [Display(Name = "Technician")]
        public string TechnicianId { get; set; }
        [Display(Name = "Driver")]
        public string? DriverId { get; set; }
        public int TicketId { get; set; }

        #region Mapping

        public static explicit operator AddAppointmentViewModel(Appointment model)
        {
            var viewmodel = new AddAppointmentViewModel
            {
                StartDateTime = (DateTime)model.StartDateTime,
                TechnicianId = model.TechnicianId,
                DriverId = model.DriverId,
                TicketId = model.Id,
            };

            return viewmodel;
        }

        public static explicit operator Appointment(AddAppointmentViewModel viewModel)
        {
            return new Appointment
            {
                StartDateTime = viewModel.StartDateTime,
                TechnicianId = viewModel.TechnicianId,
                DriverId = viewModel.DriverId,
                TicketId = viewModel.TicketId,
            };
        }

        #endregion
    }
}
