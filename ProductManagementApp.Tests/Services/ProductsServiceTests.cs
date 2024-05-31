using System.Collections.Generic;
using System.IO;
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
            
            string userId = "user1"; 
            var products = new List<Product>
            {
                
            }.AsQueryable();

            
            var mockSet = new Mock<DbSet<Product>>();
            mockSet.As<IQueryable<Product>>().Setup(m => m.Provider).Returns(products.Provider);
            mockSet.As<IQueryable<Product>>().Setup(m => m.Expression).Returns(products.Expression);
            mockSet.As<IQueryable<Product>>().Setup(m => m.ElementType).Returns(products.ElementType);
            mockSet.As<IQueryable<Product>>().Setup(m => m.GetEnumerator()).Returns(products.GetEnumerator());
            _dbContextMock.Setup(p => p.Products).Returns(mockSet.Object);

            var result = await _productsService.GetAllProductsAsync(1, 10, userId);

            Assert.Equal(10, result.Count);
            
        }

        
    }
}
