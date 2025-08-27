using DALProject.Models;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;

namespace PLProj.Models
{
    public class CartVM
    {
        public int Id { get; set; }
        [Display(Name = "Product")]
        public int ProductId { get; set; }
        [ValidateNever]
        public Product? Product { get; set; }
        [Range(1, 100, ErrorMessage = "You must Enter value between 1 to 100")]
        public int count { get; set; }
        public string? UserId { get; set; }
        [ValidateNever]
        public AppUser? User { get; set; }

        #region Mapping
        public static explicit operator CartVM(ShoppingCart model)
        {
            return new CartVM
            {
                Id = model.Id,
                ProductId = model.ProductId,
                Product = model?.Product,
                count = model.count,
                UserId = model.UserId,    
                User = model?.User
            };
        }

        public static explicit operator ShoppingCart(CartVM viewModel)
        {
            return new ShoppingCart
            {
                Id = viewModel.Id,
                ProductId = viewModel.ProductId,
                count = viewModel.count,
                UserId = viewModel.UserId,
            };
        }
        #endregion

    }
}
