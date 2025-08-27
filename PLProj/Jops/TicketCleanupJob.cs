using BLLProject.Interfaces;
using BLLProject.Specifications;
using DALProject.Models;
using System;
using System.Linq;

namespace PLProj.Jops
{
    public class TicketCleanupJob
    {
        private readonly IUnitOfWork _unitOfWork;

        public TicketCleanupJob(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public void RemoveUnpaidTickets()
        {
            var twoDaysAgo = DateTime.UtcNow.AddDays(-2);

            var unpaidTickets = _unitOfWork.Repository<Ticket>()
                .GetAllWithSpec(new BaseSpecification<Ticket>(
                    t => t.PaymentStatus == null && t.CreatedAt <= twoDaysAgo));

            if (unpaidTickets.Any())
            {
                foreach (var ticket in unpaidTickets)
                {
                    _unitOfWork.Repository<Ticket>().Delete(ticket);
                }

                _unitOfWork.Complete();
            }
        }
    }

}
