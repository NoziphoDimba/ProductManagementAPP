using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using ProductManagementAPP.Models;

namespace ProductManagementAPP.Services
{
    public interface ICategoriesService
    {
        Task<List<Category>> GetAllCategoriesAsync(string userId);
        Task<Category> GetCategoryByIdAsync(int id);
        Task AddCategoryAsync(Category category, string userId, string createdBy);
        Task UpdateCategoryAsync(Category category);
        Task DeleteCategoryAsync(int id);
    }

    public class CategoriesService : ICategoriesService
    {
        private readonly ProductsDbContext _context;

        public CategoriesService(ProductsDbContext context)
        {
            _context = context;
        }

        public async Task<List<Category>> GetAllCategoriesAsync(string userId)
        {
            return await _context.Categories
                .Where(c => c.UserId == userId)
                .ToListAsync();
        }


        public async Task<Category> GetCategoryByIdAsync(int id)
        {
            return await _context.Categories.FindAsync(id);
        }

        public async Task AddCategoryAsync(Category category, string userId, string createdBy)
        {
            category.CreatedBy = createdBy;
            category.UserId = userId; // Save the user ID
            category.DateCreated = DateTime.Now;
            _context.Categories.Add(category);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateCategoryAsync(Category category)
        {
            _context.Categories.Update(category);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteCategoryAsync(int id)
        {
            var category = await _context.Categories.FindAsync(id);
            if (category != null)
            {
                _context.Categories.Remove(category);
                await _context.SaveChangesAsync();
            }
        }
    }
}
