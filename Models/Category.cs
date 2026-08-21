namespace TechStore.Models
{
    public class Category : Entity<Guid>
    {
        public string Name { get; set; }

        public ICollection<CategoryProduct> CategoryProducts { get; set; } = new List<CategoryProduct>();

    }
}