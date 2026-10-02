using TechStore.Models;
using Microsoft.AspNetCore.Http;

namespace TechStore.Interfaces
{
    public interface IProductsService :
        IEntityReaderService<Product, Guid>,
        IEntityWriterService<Product, Guid>,
        IEntityLifecycleService<Product, Guid>,
        IProductQueries,
        IProductLifecycle
    {
    }
}
