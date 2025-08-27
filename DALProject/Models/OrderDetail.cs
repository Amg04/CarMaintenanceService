using DALProject.Models.BaseClasses;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations.Schema;

namespace DALProject.Models
{
    public class OrderDetail: IAllowedEntity
    {
        public int Id { get; set; }
        public int OrderHeaderId { get; set; }
        [ValidateNever]
        [ForeignKey(nameof(OrderHeaderId))]
        public OrderHeader OrderHeader { get; set; } = null!;
        public int ProductId { get; set; }
        [ValidateNever]
        [ForeignKey(nameof(ProductId))]
        public  Product Product { get; set; } = null!;
        public int Count { get; set; }
        public decimal Price { get; set; }
    }
}
