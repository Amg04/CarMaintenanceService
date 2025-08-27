using DALProject.Models;
using System.Collections.Generic;

namespace PLProj.Models
{
    public class ShoppingCartVM
    {
        public IEnumerable<ShoppingCart> CartsList { get; set; }
        public decimal TotalCarts { get; set; }
        public  virtual OrderHeader OrderHeader { get; set; }
    }
}
