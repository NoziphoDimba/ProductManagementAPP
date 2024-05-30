using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace ProductManagementAPP.Models
{
    public class ProductsDbContext : IdentityDbContext<ApplicationUser>
    {
        public ProductsDbContext(DbContextOptions<ProductsDbContext> options) : base(options)
        {
        }

        public DbSet<Category> Categories { get; set; }
        public DbSet<Product> Products { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Product>()
                .HasKey(p => p.ProductId);

            modelBuilder.Entity<Category>()
                .HasKey(c => c.CategoryId);

            modelBuilder.Entity<Category>()
                .HasIndex(c => c.Name)
                .IsUnique(); 
            modelBuilder.Entity<Product>()
                .Property(p => p.DateCreated)
                .HasDefaultValueSql("GETUTCDATE()"); 

            modelBuilder.Entity<Category>()
                .Property(c => c.DateCreated)
                .HasDefaultValueSql("GETUTCDATE()"); 

            modelBuilder.Entity<Product>()
                .Property(p => p.CreatedBy)
                .IsRequired();

            modelBuilder.Entity<Category>()
                .Property(c => c.CreatedBy)
                .IsRequired();

            modelBuilder.Entity<Product>()
                .HasOne(p => p.User)
                .WithMany()
                .HasForeignKey(p => p.UserId)
                .OnDelete(DeleteBehavior.NoAction); 

            modelBuilder.Entity<Category>()
                .HasOne(c => c.User)
                .WithMany()
                .HasForeignKey(c => c.UserId)
                .OnDelete(DeleteBehavior.NoAction); 

            modelBuilder.Entity<Product>()
                .Property(p => p.Price)
                .HasColumnType("decimal(18,2)"); 
        }

    }
}
