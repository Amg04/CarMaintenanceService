using DALProject.Models;
using System;
using System.ComponentModel.DataAnnotations;

namespace PLProj.Models
{
    public class FinishAppointmentViewModel
    {
        public int Id { get; set; }
        [Required]
        [Display(Name = "End Date & Time")]
        public DateTime? EndDateTime { get; set; }
        [Required]
        [Display(Name = "Partial Report")]
        public string PartialReport { get; set; }
        [Display(Name = "Final Report")]
        public string? FinalReport { get; set; }
        public bool IsLastAppointment { get; set; }
        public int TicketId { get; set; }
       
        #region Mapping

        public static explicit operator FinishAppointmentViewModel(Appointment model)
        {
            return new FinishAppointmentViewModel
            {
                Id = model.Id,
                EndDateTime = model.EndDateTime,
                PartialReport = model.PartialReport,
                FinalReport = model.Ticket.FinalReport,
                TicketId = model.TicketId
            };
        }
        #endregion
    }
}

