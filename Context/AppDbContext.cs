using CatalogAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace CatalogAPI.Context
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {}
        public DbSet<Product> Products { get; set; }

        public DbSet<Category> Categories { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            //Category
            modelBuilder.Entity<Category>().HasKey(c => c.CategoryId);
            modelBuilder.Entity<Category>().Property(c => c.Name)
                                           .HasMaxLength(100)
                                           .IsRequired();
            modelBuilder.Entity<Category>().Property(c => c.Description)
                                           .HasMaxLength(200);
            
            //Product
            modelBuilder.Entity<Product>().HasKey(p => p.ProductId);
            modelBuilder.Entity<Product>().Property(p => p.Name)
                                           .HasMaxLength(100)
                                           .IsRequired();
            modelBuilder.Entity<Product>().Property(p => p.Price).HasPrecision(14, 2)
                                           .IsRequired();

            //Relationship
            modelBuilder.Entity<Product>(c => c.HasOne<Category>(p => p.Category)
                                              .WithMany(p => p.Products)
                                              .HasForeignKey(p => p.CategoryId));


        }

    }
    
}
