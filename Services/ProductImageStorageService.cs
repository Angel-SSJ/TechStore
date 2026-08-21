using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using TechStore.Interfaces.Services;
using TechStore.Models;

namespace TechStore.Services
{
    public class ProductImageStorageService : StorageService, IProductImageStorageService
    {
        protected override string BaseDirectory => "images/products";
        protected override string[] AllowedExtensions => new[] { ".webp", ".jpg", ".jpeg", ".png" };
        protected override long MaxFileSize => 5 * 1024 * 1024; // 5MB

        public ProductImageStorageService(IWebHostEnvironment environment) : base(environment)
        {
        }

        public virtual async Task<ProductImage> SaveProductImageAsync(Guid productId, IFormFile imageFile)
        {
            ValidateFile(imageFile);

            string targetDirectory = GetAbsoluteDirectoryPath(productId.ToString());

            if (!Directory.Exists(targetDirectory))
            {
                Directory.CreateDirectory(targetDirectory);
            }

            int imageNumber = GetNextFileNumber(targetDirectory);

            string extension = Path.GetExtension(imageFile.FileName).ToLower();
            if (string.IsNullOrEmpty(extension))
            {
                extension = ".webp";
            }

            string fileName = $"{imageNumber:D2}{extension}";
            string filePath = Path.Combine(targetDirectory, fileName);

            await SaveFileToDiskAsync(imageFile, filePath);

            var fileInfo = new FileInfo(filePath);

            var productImage = new ProductImage
            {
                ProductId = productId,
                ImagePath = $"{BaseDirectory}/{productId}/{fileName}",
                ImageNumber = imageNumber,
                OriginalFileName = imageFile.FileName,
                FileSize = fileInfo.Length,
                CreatedAt = DateTime.UtcNow,
                IsActive = true
            };

            return productImage;
        }

        public virtual async Task<IList<ProductImage>> SaveProductImagesAsync(Guid productId, IEnumerable<IFormFile> imageFiles)
        {
            if (imageFiles == null)
            {
                return new List<ProductImage>();
            }

            var savedImages = new List<ProductImage>();
            foreach (var file in imageFiles)
            {
                if (file != null && file.Length > 0)
                {
                    var savedImage = await SaveProductImageAsync(productId, file);
                    savedImages.Add(savedImage);
                }
            }

            return savedImages;
        }

        public virtual void DeleteProductImage(ProductImage productImage)
        {
            if (productImage == null || string.IsNullOrWhiteSpace(productImage.ImagePath))
            {
                return;
            }

            DeleteFile(productImage.ImagePath);
        }

        public virtual void DeleteAllProductImages(Guid productId)
        {
            DeleteDirectory(productId.ToString());
        }
    }
}
