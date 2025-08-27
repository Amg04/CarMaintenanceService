using DALProject.Models;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.Collections.Generic;

namespace PLProj.Models
{
    public class OrderVM
    {
        public OrderHeader OrderHeader { get; set; }
        [ValidateNever]
        public IEnumerable<OrderDetail> OrderDetials { get; set; }
    }
}
