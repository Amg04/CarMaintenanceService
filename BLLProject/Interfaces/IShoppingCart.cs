using DALProject.Models;
namespace BLLProject.Interfaces
{
    public interface IShoppingCart : IGenericRepository<ShoppingCart>
    {
        int IncreaseCount(ShoppingCart cart, int count);
        int DecreaseCount(ShoppingCart cart, int count);
    }
}
