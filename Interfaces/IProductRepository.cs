using TechStore.Models;

namespace TechStore.Interfaces
{
    public interface IProductsRepository :
        IEntityReaderRepository<Product, Guid>,
        IEntityWriterRepository<Product, Guid>,
        IEntityLifecycleRepository<Product, Guid>
    {
        Task<Product?> GetByIdWithDetailsAsync(Guid id);
    }
}
