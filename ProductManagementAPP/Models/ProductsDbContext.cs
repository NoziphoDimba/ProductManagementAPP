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

            // Configure primary keys
            modelBuilder.Entity<Product>()
                .HasKey(p => p.ProductId);

            modelBuilder.Entity<Category>()
                .HasKey(c => c.CategoryId);

            // Additional configuration for the Category entity if needed
            // For example, setting up unique constraints or indexes
            modelBuilder.Entity<Category>()
                .HasIndex(c => c.Name)
                .IsUnique(); // Ensures category names are unique

            // Configure default values for the new fields
            modelBuilder.Entity<Product>()
                .Property(p => p.DateCreated)
                .HasDefaultValueSql("GETUTCDATE()"); // Default to current UTC date

            modelBuilder.Entity<Category>()
                .Property(c => c.DateCreated)
                .HasDefaultValueSql("GETUTCDATE()"); // Default to current UTC date

            // Configure required fields
            modelBuilder.Entity<Product>()
                .Property(p => p.CreatedBy)
                .IsRequired();

            modelBuilder.Entity<Category>()
                .Property(c => c.CreatedBy)
                .IsRequired();

            // Configure the relationship between Product/Category and ApplicationUser
            modelBuilder.Entity<Product>()
                .HasOne(p => p.User)
                .WithMany()
                .HasForeignKey(p => p.UserId)
                .OnDelete(DeleteBehavior.NoAction); // Prevent cascade delete

            modelBuilder.Entity<Category>()
                .HasOne(c => c.User)
                .WithMany()
                .HasForeignKey(c => c.UserId)
                .OnDelete(DeleteBehavior.NoAction); // Prevent cascade delete

            // Specify column type for decimal property to avoid truncation issues
            modelBuilder.Entity<Product>()
                .Property(p => p.Price)
                .HasColumnType("decimal(18,2)"); // Specify precision and scale for Price
        }

    }
}
