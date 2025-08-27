using DALProject.Models;
namespace BLLProject.Interfaces
{
    public interface IOrderHeaderRepository : IGenericRepository<OrderHeader>
    {
        void UpdateStatus(int id, string OrderStatus, string? PaymentStatus);
        void UpdateStripePaymentId(int id, string sessionId, string paymentIntentId);
    }
}
