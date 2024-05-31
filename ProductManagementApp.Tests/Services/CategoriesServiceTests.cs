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

        
        var mockCategorySet = new Mock<DbSet<Category>>();
        var categories = new List<Category>
        {
            new Category { CategoryId = 1, Name = "Category 1", UserId = "user1" },
            new Category { CategoryId = 2, Name = "Category 2", UserId = "user1" }
        }.AsQueryable();

   
        mockCategorySet.As<IQueryable<Category>>().Setup(m => m.Provider).Returns(categories.Provider);
        mockCategorySet.As<IQueryable<Category>>().Setup(m => m.Expression).Returns(categories.Expression);
        mockCategorySet.As<IQueryable<Category>>().Setup(m => m.ElementType).Returns(categories.ElementType);
        mockCategorySet.As<IQueryable<Category>>().Setup(m => m.GetEnumerator()).Returns(categories.GetEnumerator());

    
        _mockContext.Setup(c => c.Categories).Returns(mockCategorySet.Object);
    }

    [Fact]
    public async Task GetAllCategoriesAsync_ReturnsAllCategories()
    {
     
        string userId = "user1";

        var categories = await _service.GetAllCategoriesAsync(userId);

        Assert.Equal(2, categories.Count());
        Assert.Contains(categories, c => c.Name == "Category 1");
        Assert.Contains(categories, c => c.Name == "Category 2");
    }
}
