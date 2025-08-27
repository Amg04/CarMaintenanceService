using DALProject.Models;
namespace BLLProject.Interfaces
{
    public interface ITicketRepository : IGenericRepository<Ticket>
    {
        void UpdateStatus(int id , string PaymentStatus);
        void UpdateStripePaymentId(int id, string sessionId, string paymentIntentId);
    }
}
