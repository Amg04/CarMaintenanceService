using DALProject.Models.BaseClasses;
using System;
namespace BLLProject.Interfaces
{
    public interface IUnitOfWork : IDisposable
    {
        IGenericRepository<T> Repository<T>() where T : class, IAllowedEntity;
        IShoppingCart ShoppingCart { get; }
        IOrderHeaderRepository OrderHeaderRepository { get; }
        ITicketRepository TicketRepository { get; }
        int Complete();
    }
}
