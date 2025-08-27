using System;
using System.ComponentModel.DataAnnotations;

namespace DALProject.Models
{
	public class TicketViewModelCustomer 
	{
        public int Id { get; set; }
        [Required]
		[Display(Name = "Current Kilometres")]
		public long CurrentKilometres { get; set; }
		[Required]
		public string Location { get; set; }
		[Required]
		[Display(Name = "Plate Number Of Cars ")]
		public int CarId { get; set; }
		[Display(Name = "Service")]
		public int ServiceId { get; set; }
		public StateType stateType { get; set; }
		public DateTime CreatedAt { get; set; }

        #region Mapping

        public static explicit operator TicketViewModelCustomer(Ticket model)
		{
			return new TicketViewModelCustomer
			{
				Id = model.Id,
				CurrentKilometres = model.CurrentKilometres,
				Location = model.Location,
				CarId = model.CarId,
				ServiceId = model.ServiceId,
				stateType = model.stateType,
                CreatedAt = model.CreatedAt
            };
		}

		public static explicit operator Ticket(TicketViewModelCustomer viewModel)
		{
			return new Ticket
			{
				Id = viewModel.Id,
				CurrentKilometres = viewModel.CurrentKilometres,
				Location = viewModel.Location,
				CarId = viewModel.CarId,
				ServiceId = viewModel.ServiceId,
				stateType=viewModel.stateType,
                CreatedAt = viewModel.CreatedAt
            };
		}

		#endregion
	}

}

