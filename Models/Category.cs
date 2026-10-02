namespace TechStore.Models
{
    public class Category: Entity<Guid>
    {
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;


        public Category()
        {
        }

        public Category(string name, string description, bool isActive = true)
        {
            Name = name;
            Description = description;
            if (isActive)
            {
                Activate();
            }
            else
            {
                Deactivate();
            }
        }

        private readonly List<Product> _products = new();

        public IReadOnlyCollection<Product> Products => _products;

        public void UpdateDetails(string name, string description)
        {
            Name = name;
            Description = description;
            UpdatedAt = DateTime.Now;
        }

        public void ReplaceProducts(IEnumerable<Product> products)
        {
            _products.Clear();
            _products.AddRange(products);
        }
    }
}