using TechStore.Interfaces;
using TechStore.Models;

namespace TechStore.Services
{
    public class ProductImageService : IProductImageService
    {
        private readonly IProductImageStorage _storage;
        private readonly IProductImageRepository _repository;
        private readonly IImageFileValidator _validator;

        public ProductImageService(
            IProductImageStorage storage,
            IProductImageRepository repository,
            IImageFileValidator validator)
        {
            _storage = storage;
            _repository = repository;
            _validator = validator;
        }

        public async Task AddToProductAsync(Guid productId, ICollection<IFormFile> imageFiles)
        {
            if (imageFiles == null || imageFiles.Count == 0)
            {
                throw new ArgumentException("Debes seleccionar al menos una imagen.");
            }

            foreach (var imageFile in imageFiles)
            {
                _validator.Validate(imageFile);
                var storedImage = await _storage.SaveAsync(productId, imageFile);

                var productImage = new ProductImage(
                    productId,
                    storedImage.ImagePath,
                    storedImage.ImageNumber,
                    imageFile.FileName,
                    storedImage.FileSize);

                await _repository.AddAsync(productImage);
            }
        }

        public async Task RemoveAsync(Guid imageId)
        {
            var productImage = await _repository.GetByIdAsync(imageId);
            if (productImage == null)
            {
                throw new InvalidOperationException("Imagen no encontrada.");
            }

            await _repository.RemoveAsync(productImage);
            _storage.Delete(productImage);
        }

    }
}
    