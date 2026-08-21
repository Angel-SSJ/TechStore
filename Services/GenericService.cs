using TechStore.Interfaces;
using TechStore.Interfaces.Repositories;
using TechStore.Interfaces.Services;

namespace TechStore.Services
{
    public class GenericService<T, I> : IService<T, I> where T : class, IEntity<I>
    {
        protected readonly IRepository<T, I> _repository;

        public GenericService(IRepository<T, I> repository)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        }

        public virtual async Task<T> AddAsync(T entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException(nameof(entity));
            }

            return await _repository.AddAsync(entity);
        }

        public virtual async Task<T> UpdateAsync(T entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException(nameof(entity));
            }

            return await _repository.UpdateAsync(entity);
        }

        public virtual async Task<T> DeleteAsync(I id)
        {
            return await _repository.DeleteAsync(id);
        }

        public virtual async Task<T> RestoreAsync(I id)
        {
            return await _repository.RestoreAsync(id);
        }

        public virtual async Task<T> AlreadyExistsByIdAsync(I id)
        {
            return await _repository.AlreadyExistsByIdAsync(id);
        }

        public virtual async Task<T?> GetByIdAsync(I id)
        {
            return await _repository.GetByIdAsync(id);
        }

        public virtual async Task<IList<T>> GetAllAsync()
        {
            return await _repository.GetAllAsync();
        }

        public virtual async Task<IList<T>> GetAllActiveAsync()
        {
            return await _repository.GetAllActiveAsync();
        }

        public virtual async Task<IList<T>> GetAllInactiveAsync()
        {
            return await _repository.GetAllInactiveAsync();
        }
    }
}
