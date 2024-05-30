using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Moq;
using ProductManagementAPP.Models;
using ProductManagementAPP.Services;
using Xunit;

namespace ProductManagementApp.Tests.Services
{
    public class ProductsServiceTests
    {
        private readonly Mock<ProductsDbContext> _dbContextMock;
        private readonly ProductsService _productsService;

        public ProductsServiceTests()
        {
            _dbContextMock = new Mock<ProductsDbContext>();
            _productsService = new ProductsService(_dbContextMock.Object);
        }

        [Fact]
        public async Task GetAllProductsAsync_ReturnsPagedProducts()
        {
            // Arrange
            var products = new List<Product>
            {
                new Product { ProductId = 1, Name = "Laptop" },
                new Product { ProductId = 2, Name = "Book" },
                new Product { ProductId = 3, Name = "Phone" },
                new Product { ProductId = 4, Name = "Tablet" },
                new Product { ProductId = 5, Name = "Monitor" },
                new Product { ProductId = 6, Name = "Keyboard" },
                new Product { ProductId = 7, Name = "Mouse" },
                new Product { ProductId = 8, Name = "Speaker" },
                new Product { ProductId = 9, Name = "Headphone" },
                new Product { ProductId = 10, Name = "Printer" },
                new Product { ProductId = 11, Name = "Camera" }
            }.AsQueryable();

            var mockSet = new Mock<DbSet<Product>>();
            mockSet.As<IQueryable<Product>>().Setup(m => m.Provider).Returns(products.Provider);
            mockSet.As<IQueryable<Product>>().Setup(m => m.Expression).Returns(products.Expression);
            mockSet.As<IQueryable<Product>>().Setup(m => m.ElementType).Returns(products.ElementType);
            mockSet.As<IQueryable<Product>>().Setup(m => m.GetEnumerator()).Returns(products.GetEnumerator());

            _dbContextMock.Setup(p => p.Products).Returns(mockSet.Object);

            // Act
            var result = await _productsService.GetAllProductsAsync(1, 10);

            // Assert
            Assert.Equal(10, result.Count);
            Assert.Equal("Laptop", result[0].Name);
            Assert.Equal("Printer", result[9].Name);

            // Act for second page
            var resultPage2 = await _productsService.GetAllProductsAsync(2, 10);

            // Assert for second page
            Assert.Single(resultPage2);
            Assert.Equal("Camera", resultPage2[0].Name);
        }

        [Fact]
        public async Task AddProductAsync_AddsProductSuccessfully()
        {
            // Arrange
            var product = new Product { ProductId = 1, Name = "Laptop" };
            var mockSet = new Mock<DbSet<Product>>();

            _dbContextMock.Setup(p => p.Products).Returns(mockSet.Object);

            // Act
            var result = await _productsService.AddProductAsync(product);

            // Assert
            mockSet.Verify(m => m.Add(It.IsAny<Product>()), Times.Once());
            _dbContextMock.Verify(m => m.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once());
            Assert.Equal(product, result);
        }

        [Fact]
        public async Task UpdateProductAsync_UpdatesProductSuccessfully()
        {
            // Arrange
            var product = new Product { ProductId = 1, Name = "Laptop" };
            var mockSet = new Mock<DbSet<Product>>();
            _dbContextMock.Setup(p => p.Products).Returns(mockSet.Object);
            _dbContextMock.Setup(p => p.Products.FindAsync(It.IsAny<int>())).ReturnsAsync(product);

            // Act
            product.Name = "Updated Laptop";
            var result = await _productsService.UpdateProductAsync(product);

            // Assert
            Assert.Equal("Updated Laptop", result.Name);
            _dbContextMock.Verify(m => m.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once());
        }

        [Fact]
        public async Task DeleteProductAsync_DeletesProductSuccessfully()
        {
            // Arrange
            var product = new Product { ProductId = 1, Name = "Laptop" };
            var mockSet = new Mock<DbSet<Product>>();
            _dbContextMock.Setup(p => p.Products).Returns(mockSet.Object);
            _dbContextMock.Setup(p => p.Products.FindAsync(It.IsAny<int>())).ReturnsAsync(product);

            // Act
            var result = await _productsService.DeleteProductAsync(1);

            // Assert
            Assert.True(result);
            mockSet.Verify(m => m.Remove(It.IsAny<Product>()), Times.Once());
            _dbContextMock.Verify(m => m.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once());
        }
    }
}
