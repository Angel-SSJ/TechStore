namespace TechStore.Interfaces
{
    public interface IEntityWriterService<T, I> where T : IEntity<I>
    {
        Task<T> AddAsync(T entity);
        Task<T> UpdateAsync(T entity);
    }
}
