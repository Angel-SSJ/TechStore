using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using TechStore.Interfaces;
using TechStore.Models;

namespace TechStore.Data.Storage
{
    public class LocalProductImageStorage : IProductImageStorage
    {
        private const string ImagesDirectory = "Images/Products";

        private readonly IWebHostEnvironment _environment;

        public LocalProductImageStorage(
            IWebHostEnvironment environment)
        {
            _environment = environment;
        }

        public async Task<StoredProductImage> SaveAsync(
            Guid productId,
            IFormFile imageFile)
        {
            string productImagesDirectory = Path.Combine(
                _environment.WebRootPath,
                ImagesDirectory,
                productId.ToString());

            Directory.CreateDirectory(productImagesDirectory);

            int imageNumber =
                Directory.GetFiles(productImagesDirectory).Length + 1;

            string extension =
                Path.GetExtension(imageFile.FileName)
                    .ToLowerInvariant();

            if (string.IsNullOrEmpty(extension))
            {
                extension = ".webp";
            }

            string fileName =
                $"{imageNumber:D2}{extension}";

            string filePath = Path.Combine(
                productImagesDirectory,
                fileName);

            await using (var stream =
                new FileStream(filePath, FileMode.Create))
            {
                await imageFile.CopyToAsync(stream);
            }

            return new StoredProductImage(
                $"{ImagesDirectory}/{productId}/{fileName}",
                imageNumber,
                new FileInfo(filePath).Length);
        }

        public void Delete(ProductImage productImage)
        {
            string filePath = Path.Combine(
                _environment.WebRootPath,
                productImage.ImagePath.TrimStart('/'));

            if (File.Exists(filePath))
            {
                File.Delete(filePath);
            }

            string? directoryPath =
                Path.GetDirectoryName(filePath);

            if (directoryPath != null &&
                Directory.Exists(directoryPath) &&
                Directory.GetFiles(directoryPath).Length == 0)
            {
                Directory.Delete(directoryPath);
            }
        }
    }
}