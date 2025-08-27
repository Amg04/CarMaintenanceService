using BLLProject.Interfaces;
using DALProject.Data;
using DALProject.Models;
using System;
using System.Linq;
namespace BLLProject.Repositories
{
    public class TicketRepository : GenericRepository<Ticket>, ITicketRepository
    {
        private readonly CarAppDbContext _context;
        public TicketRepository(CarAppDbContext context) : base(context)
        {

            _context = context;
        }

        public void UpdateStatus(int id , string PaymentStatus)
        {
            var ticketfromdb = _context.Tickets.FirstOrDefault(x => x.Id == id);
            if (ticketfromdb != null)
            {
                ticketfromdb.PaymentDate = DateTime.Now;
                if (PaymentStatus != null)
                {
                    ticketfromdb.PaymentStatus = PaymentStatus;
                }
            }
        }

        public void UpdateStripePaymentId(int id, string sessionId, string paymentIntentId)
        {
            var ticketFromDb = _context.Tickets.FirstOrDefault(x => x.Id == id);
            if (ticketFromDb != null)
            {
                if (!string.IsNullOrEmpty(sessionId))
                {
                    ticketFromDb.SessionId = sessionId;
                }
                if (!string.IsNullOrEmpty(paymentIntentId))
                {
                    ticketFromDb.PaymentIntentId = paymentIntentId;
                    ticketFromDb.PaymentDate = DateTime.Now;
                }
            }
        }
    }
}
