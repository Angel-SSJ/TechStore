namespace TechStore.Models
{
    public class Product : Entity<Guid>
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }

        public int Stock {get; set; }

        public ICollection<ProductImage> Images { get; set; } = new List<ProductImage>();
        public ICollection<CategoryProduct> Categories { get; set; } = new List<CategoryProduct>();

    }
}