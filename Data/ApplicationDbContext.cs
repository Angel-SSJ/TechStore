using TechStore.Interfaces;
using TechStore.Models;
using Microsoft.EntityFrameworkCore;

namespace TechStore.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        public DbSet<Product> Products { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<CategoryProduct> CategoryProducts { get; set; }
        public DbSet<ProductImage> ProductImages { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // ==========================================
            // Configuración de Category
            // ==========================================
            modelBuilder.Entity<Category>(entity =>
            {
                entity.HasIndex(c => c.Id);
                entity.Property(c => c.Id).HasDefaultValueSql("NEWID()");
                entity.Property(c => c.CreatedAt).HasDefaultValueSql("GETUTCDATE()");
                entity.Property(c => c.IsActive).HasDefaultValue(true);
            });

            // ==========================================
            // Configuración de Product
            // ==========================================
            modelBuilder.Entity<Product>(entity =>
            {
                entity.HasIndex(p => p.Id);
                entity.Property(p => p.Id).HasDefaultValueSql("NEWID()");
                entity.Property(p => p.CreatedAt).HasDefaultValueSql("GETUTCDATE()");
                entity.Property(p => p.IsActive).HasDefaultValue(true);
                entity.Property(p => p.Price).HasPrecision(18, 2);
            });

            // ==========================================
            // Configuración de CategoryProduct (Tabla Intermedia N:M)
            // ==========================================
            modelBuilder.Entity<CategoryProduct>(entity =>
            {
                entity.HasKey(cp => new { cp.ProductId, cp.CategoryId });

                entity.Property(cp => cp.Id).HasDefaultValueSql("NEWID()");
                entity.Property(cp => cp.CreatedAt).HasDefaultValueSql("GETUTCDATE()");
                entity.Property(cp => cp.IsActive).HasDefaultValue(true);

                entity.HasOne(cp => cp.Product)
                    .WithMany(p => p.Categories)
                    .HasForeignKey(cp => cp.ProductId);

                entity.HasOne(cp => cp.Category)
                    .WithMany(c => c.CategoryProducts)
                    .HasForeignKey(cp => cp.CategoryId);
            });

            // ==========================================
            // Configuración de ProductImage
            // ==========================================
            modelBuilder.Entity<ProductImage>(entity =>
            {
                entity.HasIndex(pi => pi.ProductId);
                entity.Property(pi => pi.Id).HasDefaultValueSql("NEWID()");
                entity.Property(pi => pi.CreatedAt).HasDefaultValueSql("GETUTCDATE()");
                entity.Property(pi => pi.IsActive).HasDefaultValue(true);

                entity.Property(pi => pi.ImagePath)
                    .IsRequired()
                    .HasMaxLength(500);

                entity.Property(pi => pi.OriginalFileName)
                    .IsRequired()
                    .HasMaxLength(255);

                entity.HasOne(pi => pi.Product)
                    .WithMany(p => p.Images)
                    .HasForeignKey(pi => pi.ProductId)
                    .OnDelete(DeleteBehavior.Cascade);
            });
        }

        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            ApplyAuditing();
            return base.SaveChangesAsync(cancellationToken);
        }

        public override int SaveChanges()
        {
            ApplyAuditing();
            return base.SaveChanges();
        }

        private void ApplyAuditing()
        {
            var entries = ChangeTracker.Entries()
                .Where(e => e.Entity is IEntity<Guid> && (e.State == EntityState.Added || e.State == EntityState.Modified));

            foreach (var entry in entries)
            {
                if (entry.Entity is Entity<Guid> entity)
                {
                    if (entry.State == EntityState.Added)
                    {
                        if (entity.Id == Guid.Empty)
                        {
                            entity.Id = Guid.NewGuid();
                        }
                        if (entity.CreatedAt == default)
                        {
                            entity.CreatedAt = DateTime.UtcNow;
                        }
                        entity.IsActive = true;
                    }
                    else if (entry.State == EntityState.Modified)
                    {
                        entity.UpdatedAt = DateTime.UtcNow;
                    }
                }
            }
        }
    }
}