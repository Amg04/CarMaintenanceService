using DALProject.Models;
using System;
using System.ComponentModel.DataAnnotations;

namespace PLProj.Models
{
    public class EditTickerViewModel
    {
        public int id { get; set; }
        [Display(Name = "Start Date & Time")]
        public DateTime? StartDataTime { get; set; }
        [Display(Name = "End Date & Time")]
        public DateTime? EndDateTime { get; set; }
        [Required]
        [Display(Name = "State")]
        public StateType StateType { get; set; }
        [Display(Name = "Final Report")]
        public string? FinalReport { get; set; }

        #region Mapping
        public static explicit operator EditTickerViewModel(Ticket model)
        {
            var viewmodel = new EditTickerViewModel()
            {
                id = model.Id,
                StartDataTime = model.StartDateTime,
                EndDateTime = model.EndDateTime,
                StateType = model.stateType,
                FinalReport = model.FinalReport,
            };

            return viewmodel;
        }

        public static explicit operator Ticket(EditTickerViewModel viewModel)
        {
            return new Ticket()
            {
                Id = viewModel.id,
                StartDateTime = viewModel.StartDataTime,
                EndDateTime = viewModel.EndDateTime,
                stateType = viewModel.StateType,
                FinalReport = viewModel?.FinalReport,
            };
        }
        #endregion
    }
}
