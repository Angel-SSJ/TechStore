namespace TechStore.Models
{
    public class Product: Entity<Guid>
    {

        
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public int Stock { get; set; }
        public bool Featured { get; set; }

        private readonly List<Category> _categories = new();
        private readonly List<ProductImage> _images = new();

        public IReadOnlyCollection<Category> Categories => _categories;
        public IReadOnlyCollection<ProductImage> Images => _images;



        public Product ()
        {
        }

        public Product(
            string name,
            string description,
            decimal price,
            int stock,
            bool featured,
            bool isActive = true
        )
        {
            UpdateDetails(name, description, price, stock, featured);
            if (isActive)
            {
                Activate();
            }
            else
            {
                Deactivate();
            }
            
        }

        public void UpdateDetails(string name, string description, decimal price, int stock, bool featured)
        {
            Name = name;
            Description = description;
            Price = price;
            Stock = stock;
            Featured = featured;
            UpdatedAt = DateTime.Now;
        }

        public void ReplaceCategories(IEnumerable<Category> categories)
        {
            _categories.Clear();
            _categories.AddRange(categories);
        }

        public void AddImage(ProductImage image)
        {
            _images.Add(image);
        }

        public void RemoveImage(ProductImage image)
        {
            _images.Remove(image);
        }
        
    }
}