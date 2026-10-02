using TechStore.DTOs;
using TechStore.Models;

namespace TechStore.Interfaces
{
    public interface ICategoryService :
        IEntityReaderService<Category, Guid>,
        IEntityWriterService<Category, Guid>,
        IEntityLifecycleService<Category, Guid>,
        ICategoryQueries,
        ICategoryLifecycle,
        ICategoryApplicationService
    {
    }
}
