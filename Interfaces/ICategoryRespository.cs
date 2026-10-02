using TechStore.Models;

namespace TechStore.Interfaces
{
    public interface ICategoryRespository :
        IEntityReaderRepository<Category, Guid>,
        IEntityWriterRepository<Category, Guid>,
        IEntityLifecycleRepository<Category, Guid>
    {
        Task<Category?> GetByIdWithProductsAsync(Guid id);
        Task<bool> HasProductsAsync(Guid id);
        Task<bool> NameExistsAsync(string name, Guid? excludingId = null);
    }
}
