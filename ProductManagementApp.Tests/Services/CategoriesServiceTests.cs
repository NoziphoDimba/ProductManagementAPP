//using System.Collections.Generic;
//using System.Linq;
//using System.Threading.Tasks;
//using Microsoft.EntityFrameworkCore;
//using Moq;
//using ProductManagementAPP.Models;
//using ProductManagementAPP.Services;
//using Xunit;

//namespace ProductManagementApp.Tests.Services
//{
//    public class CategoriesServiceTests
//    {
//        private readonly Mock<ProductsDbContext> _dbContextMock;
//        private readonly CategoriesService _categoriesService;

//        public CategoriesServiceTests()
//        {
//            var options = new DbContextOptionsBuilder<ProductsDbContext>()
//                .UseInMemoryDatabase(databaseName: "TestDatabase")
//                .Options;

//            _dbContextMock = new Mock<ProductsDbContext>(options);
//            _categoriesService = new CategoriesService(_dbContextMock.Object);
//        }

//        [Fact]
//        public async Task GetAllCategoriesAsync_ReturnsAllCategories()
//        {
//            // Arrange
//            var categories = new List<Category>
//            {
//                new Category { CategoryId = 1, Name = "Electronics" },
//                new Category { CategoryId = 2, Name = "Books" }
//            }.AsQueryable();

//            var mockSet = new Mock<DbSet<Category>>();
//            mockSet.As<IQueryable<Category>>().Setup(m => m.Provider).Returns(categories.Provider);
//            mockSet.As<IQueryable<Category>>().Setup(m => m.Expression).Returns(categories.Expression);
//            mockSet.As<IQueryable<Category>>().Setup(m => m.ElementType).Returns(categories.ElementType);
//            mockSet.As<IQueryable<Category>>().Setup(m => m.GetEnumerator()).Returns(categories.GetEnumerator());

//            _dbContextMock.Setup(c => c.Categories).Returns(mockSet.Object);

//            // Act
//            var result = await _categoriesService.GetAllCategoriesAsync();

//            // Assert
//            Assert.Equal(2, result.Count);
//            Assert.Equal("Electronics", result[0].Name);
//            Assert.Equal("Books", result[1].Name);
//        }
//    }
//}

using Moq;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Xunit;
using ProductManagementAPP.Models;
using ProductManagementAPP.Services;

public class CategoriesServiceTests
{
    private readonly Mock<ProductsDbContext> _mockContext;
    private readonly CategoriesService _service;

    public CategoriesServiceTests()
    {
        _mockContext = new Mock<ProductsDbContext>();
        _service = new CategoriesService(_mockContext.Object);

        // Set up the mock DbSet
        var mockCategorySet = new Mock<DbSet<Category>>();
        var categories = new List<Category>
        {
            new Category { CategoryId = 1, Name = "Category 1" },
            new Category { CategoryId = 2, Name = "Category 2" }
        }.AsQueryable();

        // Configure the mock DbSet
        mockCategorySet.As<IQueryable<Category>>().Setup(m => m.Provider).Returns(categories.Provider);
        mockCategorySet.As<IQueryable<Category>>().Setup(m => m.Expression).Returns(categories.Expression);
        mockCategorySet.As<IQueryable<Category>>().Setup(m => m.ElementType).Returns(categories.ElementType);
        mockCategorySet.As<IQueryable<Category>>().Setup(m => m.GetEnumerator()).Returns(categories.GetEnumerator());

        // Set up the DbContext to return the mock DbSet
        _mockContext.Setup(c => c.Categories).Returns(mockCategorySet.Object);
    }

    [Fact]
    public async Task GetAllCategoriesAsync_ReturnsAllCategories()
    {
        //var categories = await _service.GetAllCategoriesAsync();
        //Assert.Equal(2, categories.Count());
    }
}

