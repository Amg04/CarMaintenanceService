using DALProject.Models.BaseClasses;
using System;
namespace DALProject.Models
{
    public class ProductCategory: IAllowedEntity
    {
        public int Id { get; set; }
        public required string Name { get; set; }
        public string? Description { get; set; }
        public DateTime CreatedTime { get; set; } = DateTime.Now;
    }
}
