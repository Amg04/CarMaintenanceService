using BLLProject.Interfaces;
using DALProject.Data;
using DALProject.Models.BaseClasses;
using System.Collections;
namespace BLLProject.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly CarAppDbContext dbContext;
        private Hashtable _repository;
        public IShoppingCart ShoppingCart { get; private set; }
        public IOrderHeaderRepository OrderHeaderRepository { get; private set; }
        public ITicketRepository TicketRepository { get; private set; }

        public UnitOfWork(CarAppDbContext dbContext)
        {

            this.dbContext = dbContext;

            _repository = new Hashtable();
            ShoppingCart = new ShoppingCartRepository(dbContext);
            OrderHeaderRepository = new OrderHeaderRepository(dbContext);
            TicketRepository = new TicketRepository(dbContext);

        }

        public IGenericRepository<T> Repository<T>() where T : class, IAllowedEntity
        {
            var key = typeof(T).Name;
            if (!_repository.ContainsKey(key))
            {
                _repository.Add(key, new GenericRepository<T>(dbContext));
            }
            return _repository[key] as IGenericRepository<T>;
        }

        public void Dispose()
        {
            dbContext.Dispose();
        }

        public int Complete()
        {
            return dbContext.SaveChanges();
        }


    }
}
