using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace TechStore.Models.DTOs
{
    public class CreateProductDto
    {
        [Required(ErrorMessage = "El nombre del producto es requerido.")]
        [StringLength(150, ErrorMessage = "El nombre no puede exceder 150 caracteres.")]
        public string Name { get; set; } = string.Empty;

        public string Description { get; set; } = string.Empty;

        [Range(0, double.MaxValue, ErrorMessage = "El precio debe ser mayor o igual a 0.")]
        public decimal Price { get; set; }

        [Range(0, int.MaxValue, ErrorMessage = "El stock debe ser mayor o igual a 0.")]
        public int Stock { get; set; }

        public List<Guid> CategoryIds { get; set; } = new();

        public List<IFormFile>? Images { get; set; }
    }

    public class UpdateProductDto
    {
        [Required(ErrorMessage = "El nombre del producto es requerido.")]
        [StringLength(150, ErrorMessage = "El nombre no puede exceder 150 caracteres.")]
        public string Name { get; set; } = string.Empty;

        public string Description { get; set; } = string.Empty;

        [Range(0, double.MaxValue, ErrorMessage = "El precio debe ser mayor o igual a 0.")]
        public decimal Price { get; set; }

        [Range(0, int.MaxValue, ErrorMessage = "El stock debe ser mayor o igual a 0.")]
        public int Stock { get; set; }

        public List<Guid>? CategoryIds { get; set; }

        public List<IFormFile>? NewImages { get; set; }
    }

    public class ProductDetailDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public int Stock { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public List<ProductCategoryDto> Categories { get; set; } = new();
        public List<ProductImageDto> Images { get; set; } = new();
    }

    public class ProductCategoryDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
    }

    public class ProductImageDto
    {
        public Guid Id { get; set; }
        public string ImagePath { get; set; } = string.Empty;
        public int ImageNumber { get; set; }
        public string OriginalFileName { get; set; } = string.Empty;
        public long FileSize { get; set; }
        public bool IsPrimary { get; set; }
        public string FormattedFileSize { get; set; } = string.Empty;
    }

    public class AddProductImagesDto
    {
        [Required(ErrorMessage = "Debe seleccionar al menos una imagen.")]
        public List<IFormFile> Images { get; set; } = new();
    }
}
