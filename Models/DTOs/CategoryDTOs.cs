using System.ComponentModel.DataAnnotations;

namespace TechStore.Models.DTOs
{
    public class CreateCategoryDto
    {
        [Required(ErrorMessage = "El nombre de la categoría es requerido.")]
        [StringLength(100, ErrorMessage = "El nombre no puede exceder 100 caracteres.")]
        public string Name { get; set; } = string.Empty;
    }

    public class UpdateCategoryDto
    {
        [Required(ErrorMessage = "El nombre de la categoría es requerido.")]
        [StringLength(100, ErrorMessage = "El nombre no puede exceder 100 caracteres.")]
        public string Name { get; set; } = string.Empty;
    }

    public class CategoryDetailDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public int ProductCount { get; set; }
        public List<CategoryProductItemDto> Products { get; set; } = new();
    }

    public class CategoryProductItemDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public int Stock { get; set; }
        public string? PrimaryImageUrl { get; set; }
    }
}
