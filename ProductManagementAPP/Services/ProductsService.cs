using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using ProductManagementAPP.Models;

namespace ProductManagementAPP.Services
{
    public interface IProductsService
    {
        Task<List<Product>> GetAllProductsAsync(int pageNumber, int pageSize);
        Task<Product> GetProductByIdAsync(int id);
        Task<Product> AddProductAsync(Product product);
        Task<Product> UpdateProductAsync(Product product);
        Task<bool> DeleteProductAsync(int productId);
    }

    public class ProductsService : IProductsService
    {
        private readonly ProductsDbContext _context;

        public ProductsService(ProductsDbContext context)
        {
            _context = context;
        }

        public async Task<List<Product>> GetAllProductsAsync(int pageNumber, int pageSize)
        {
            return await _context.Products
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }

        public async Task<Product> GetProductByIdAsync(int id)
        {
            return await _context.Products.FindAsync(id);
        }

        public async Task<Product> AddProductAsync(Product product)
        {
            product.ProductCode = GenerateProductCode();
            _context.Products.Add(product);
            await _context.SaveChangesAsync();
            return product;
        }

        public async Task<Product> UpdateProductAsync(Product product)
        {
            var existingProduct = await _context.Products.FindAsync(product.ProductId);
            if (existingProduct == null)
            {
                return null;
            }

            existingProduct.ProductCode = product.ProductCode;
            existingProduct.Name = product.Name;
            existingProduct.Description = product.Description;
            existingProduct.CategoryId = product.CategoryId;
            existingProduct.Price = product.Price;
            existingProduct.Image = product.Image;

            _context.Products.Update(existingProduct);
            await _context.SaveChangesAsync();

            return existingProduct;
        }

        public async Task<bool> DeleteProductAsync(int productId)
        {
            var product = await _context.Products.FindAsync(productId);
            if (product == null)
            {
                return false;
            }

            _context.Products.Remove(product);
            await _context.SaveChangesAsync();
            return true;
        }


        private string GenerateProductCode()
        {
            var lastProduct = _context.Products
                .OrderByDescending(p => p.ProductId)
                .FirstOrDefault();

            int nextNumber = lastProduct != null
                ? int.Parse(lastProduct.ProductCode.Split('-').Last()) + 1
                : 1;

            return $"{DateTime.Now:yyyyMM}-{nextNumber:000}";
        }
    }
}
