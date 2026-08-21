namespace TechStore.Models
{
    public class CategoryProduct : Entity<Guid>
    {
        public Product Product { get; set; } = null!;
        public Guid ProductId { get; set; }

        public Category Category { get; set; } = null!;
        public Guid CategoryId { get; set; }
    }
}
