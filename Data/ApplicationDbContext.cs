
using Microsoft.EntityFrameworkCore;
using TechStore.Models;

namespace TechStore.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        public DbSet<Product> Products { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<ProductImage> ProductImages { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Product>()
                .Property(product => product.Price)
                .HasPrecision(18, 2);

            modelBuilder.Entity<Category>()
                .HasMany(category => category.Products)
                .WithMany(product => product.Categories)
                .UsingEntity(join => join.ToTable("CategoryProduct"));

            modelBuilder.Entity<ProductImage>()
                .HasOne(pi => pi.Product)
                .WithMany(p => p.Images)
                .HasForeignKey(pi => pi.ProductId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<ProductImage>()
                .Property(bi => bi.ImagePath)
                .IsRequired()
                .HasMaxLength(500);

            modelBuilder.Entity<ProductImage>()
                .Property(bi => bi.OriginalFileName)
                .HasMaxLength(255);

            modelBuilder.Entity<ProductImage>()
                .HasIndex(bi => bi.ProductId);

        }

    }
}