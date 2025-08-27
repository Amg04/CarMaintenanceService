using DALProject.Models.BaseClasses;
namespace DALProject.Models
{
    public class ModelPart : IAllowedEntity
    {
        public int ModelId { get; set; }
        public Model Model { get; set; } = null!;

        public int ProductId { get; set; }
        public Product Product { get; set; } = null!;
    }
}
