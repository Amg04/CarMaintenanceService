using BLLProject.Interfaces;
using DALProject.Data;
using DALProject.Models;
using System;
using System.Linq;
namespace BLLProject.Repositories
{
    public class OrderHeaderRepository : GenericRepository<OrderHeader>, IOrderHeaderRepository
    {
        private readonly CarAppDbContext _context;
        public OrderHeaderRepository(CarAppDbContext context) : base(context)
        {
            _context = context;
        }

        public void UpdateStatus(int id, string OrderStatus, string? PaymentStatus)
        {
            var orderfromdb = _context.OrderHeaders.FirstOrDefault(x => x.Id == id);
            if (orderfromdb != null)
            {
                orderfromdb.OrderStatus = OrderStatus;
                orderfromdb.PaymentDate = DateTime.Now;
                if (PaymentStatus != null)
                {
                    orderfromdb.PaymentStatus = PaymentStatus;
                }
            }
        }

        public void UpdateStripePaymentId(int id, string sessionId, string paymentIntentId)
        {
            var orderFromDb = _context.OrderHeaders.FirstOrDefault(x => x.Id == id);
            if (orderFromDb != null)
            {
                if (!string.IsNullOrEmpty(sessionId))
                {
                    orderFromDb.SessionId = sessionId;
                }
                if (!string.IsNullOrEmpty(paymentIntentId))
                {
                    orderFromDb.PaymentIntentId = paymentIntentId;
                    orderFromDb.PaymentDate = DateTime.Now;
                }
            }
        }
    }
}
