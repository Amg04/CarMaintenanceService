using DALProject.Models.BaseClasses;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace DALProject.Models
{
    public class ShoppingCart : IAllowedEntity
    {
        public int Id { get; set; }
        public int ProductId { get; set; }
        [ValidateNever]
        public Product Product { get; set; } = null!;
        public int count { get; set; }
        public string UserId { get; set; }
        [ValidateNever]
        public AppUser User { get; set; } = null!;
    }
}
