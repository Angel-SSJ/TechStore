using TechStore.Interfaces;

namespace TechStore.Models
{
    public class Entity<T> : IEntity<T>
    {
        public T Id { get; set; } = default!;
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public DateTime? DeletedAt { get; set; }
        public bool IsActive { get; set; } = true;

        public virtual void Deactivate()
        {
            IsActive = false;
            DeletedAt = DateTime.UtcNow;
            UpdatedAt = DateTime.UtcNow;
        }

        public virtual void Activate()
        {
            IsActive = true;
            DeletedAt = null;
            UpdatedAt = DateTime.UtcNow;
        }
    }
}
