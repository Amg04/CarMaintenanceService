using DALProject.Models.BaseClasses;

namespace DALProject.Models
{
    public class Brand : IAllowedEntity
    {
        public int Id { get; set; }
        public required string Name { get; set; }
    }
}

