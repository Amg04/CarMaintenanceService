using BLLProject.Interfaces;
using DALProject.Data;
using DALProject.Models;
using System;
namespace BLLProject.Repositories
{
    public class ShoppingCartRepository : GenericRepository<ShoppingCart>, IShoppingCart
    {
        public CarAppDbContext _context;
        public ShoppingCartRepository(CarAppDbContext context) : base(context)
        {
            _context = context;
        }

        public int DecreaseCount(ShoppingCart cart, int count)
        {
            cart.count = Math.Max(0, cart.count - count);
            return cart.count;
        }

        public int IncreaseCount(ShoppingCart cart, int count)
        {
            cart.count = Math.Max(0, cart.count + count);
            return cart.count;
        }
    }
}
