using Microsoft.EntityFrameworkCore;
using TechStore.Data;
using TechStore.Interfaces;
using TechStore.Interfaces.Repositories;
using TechStore.Models;

namespace TechStore.Data.Repositories
{
    public class GenericRepository<T, I> : IRepository<T, I> where T : class, IEntity<I>
    {
        protected readonly ApplicationDbContext _context;
        protected readonly DbSet<T> _dbSet;

        public GenericRepository(ApplicationDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _dbSet = _context.Set<T>();
        }

        public virtual async Task<T> AddAsync(T entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException(nameof(entity));
            }

            if (entity.CreatedAt == default)
            {
                entity.CreatedAt = DateTime.UtcNow;
            }
            entity.IsActive = true;

            await _dbSet.AddAsync(entity);
            await _context.SaveChangesAsync();
            return entity;
        }

        public virtual async Task<T> UpdateAsync(T entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException(nameof(entity));
            }

            entity.UpdatedAt = DateTime.UtcNow;

            _dbSet.Update(entity);
            await _context.SaveChangesAsync();
            return entity;
        }

        public virtual async Task<T> DeleteAsync(I id)
        {
            var entity = await AlreadyExistsByIdAsync(id);

            if (entity is Entity<I> baseEntity)
            {
                baseEntity.Deactivate();
            }
            else
            {
                entity.IsActive = false;
                entity.DeletedAt = DateTime.UtcNow;
                entity.UpdatedAt = DateTime.UtcNow;
            }

            _dbSet.Update(entity);
            await _context.SaveChangesAsync();
            return entity;
        }

        public virtual async Task<T> RestoreAsync(I id)
        {
            var entity = await _dbSet.FirstOrDefaultAsync(e => e.Id!.Equals(id));
            if (entity == null)
            {
                throw new KeyNotFoundException($"No se encontró ninguna entidad con el ID '{id}'.");
            }

            if (entity is Entity<I> baseEntity)
            {
                baseEntity.Activate();
            }
            else
            {
                entity.IsActive = true;
                entity.DeletedAt = null;
                entity.UpdatedAt = DateTime.UtcNow;
            }

            _dbSet.Update(entity);
            await _context.SaveChangesAsync();
            return entity;
        }

        public virtual async Task<T> AlreadyExistsByIdAsync(I id)
        {
            var entity = await GetByIdAsync(id);
            if (entity == null)
            {
                throw new KeyNotFoundException($"No se encontró ninguna entidad con el ID '{id}'.");
            }
            return entity;
        }

        public virtual async Task<T?> GetByIdAsync(I id)
        {
            return await _dbSet.FirstOrDefaultAsync(e => e.Id!.Equals(id));
        }

        public virtual async Task<IList<T>> GetAllAsync()
        {
            return await _dbSet.ToListAsync<T>();
        }

        public virtual async Task<IList<T>> GetAllActiveAsync()
        {
            return await _dbSet.Where(e => e.IsActive).ToListAsync<T>();
        }

        public virtual async Task<IList<T>> GetAllInactiveAsync()
        {
            return await _dbSet.Where(e => !e.IsActive).ToListAsync<T>();
        }
    }
}
