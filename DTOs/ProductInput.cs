using System.ComponentModel.DataAnnotations;

namespace TechStore.DTOs
{
    public sealed class ProductInput
    {
        public Guid Id { get; set; }

        [Required]
        public string Name { get; set; } = string.Empty;

        [Required]
        public string Description { get; set; } = string.Empty;

        [Range(0, double.MaxValue)]
        public decimal Price { get; set; }

        [Range(0, int.MaxValue)]
        public int Stock { get; set; }

        public bool Featured { get; set; }
        public bool IsActive { get; set; } = true;
    }
}