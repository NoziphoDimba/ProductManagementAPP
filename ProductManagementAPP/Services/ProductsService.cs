using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;
using ProductManagementAPP.Models;

namespace ProductManagementAPP.Services
{
    public interface IProductsService
    {
        Task<List<Product>> GetAllProductsAsync(int pageNumber, int pageSize, string userId);
        Task<Product> GetProductByIdAsync(int id);
        Task<Product> AddProductAsync(Product product);
        Task<Product> UpdateProductAsync(Product product);
        Task<bool> DeleteProductAsync(int productId);
        Task<bool> ProcessExcelFileAsync(Stream fileStream, int productId);
        Task<byte[]> GenerateExcelFileAsync(int productId);
        Task<int> CountProducts(string id);
    }

    public class ProductsService : IProductsService
    {
        private readonly ProductsDbContext _context;

        public ProductsService(ProductsDbContext context)
        {
            _context = context;
        }
        public async Task<int> CountProducts(string id)
        {
            return await _context.Products.Where(p => p.UserId == id).CountAsync();
        }
        public async Task<List<Product>> GetAllProductsAsync(int pageNumber, int pageSize, string userId)
        {
            return await _context.Products
                .Where(p=>p.UserId==userId)
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

        public async Task<bool> ProcessExcelFileAsync(Stream fileStream, int productId)
        {
            try
            {
                using (var package = new ExcelPackage(fileStream))
                {
                    var worksheet = package.Workbook.Worksheets.FirstOrDefault();
                    if (worksheet == null) return false;

                    var rowCount = worksheet.Dimension.Rows;
                    var products = new List<Product>();

                    for (int row = 2; row <= rowCount; row++) 
                    {
                        var product = new Product
                        {
                            ProductCode = worksheet.Cells[row, 1].Text,
                            Name = worksheet.Cells[row, 2].Text,
                            Description = worksheet.Cells[row, 3].Text,
                            CategoryId = productId, 
                            Price = decimal.Parse(worksheet.Cells[row, 4].Text),
                            Image = worksheet.Cells[row, 5].Text
                        };
                        products.Add(product);
                    }

                    await _context.Products.AddRangeAsync(products);
                    await _context.SaveChangesAsync();
                }
                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<byte[]> GenerateExcelFileAsync(int productId)
        {
            var products = await _context.Products.Where(p => p.ProductId == productId).ToListAsync();

            using (var package = new ExcelPackage())
            {
                var worksheet = package.Workbook.Worksheets.Add("Products");

                worksheet.Cells[1, 1].Value = "Product Code";
                worksheet.Cells[1, 2].Value = "Name";
                worksheet.Cells[1, 3].Value = "Description";
                worksheet.Cells[1, 4].Value = "Price";
                worksheet.Cells[1, 5].Value = "Image";

                for (int i = 0; i < products.Count; i++)
                {
                    worksheet.Cells[i + 2, 1].Value = products[i].ProductCode;
                    worksheet.Cells[i + 2, 2].Value = products[i].Name;
                    worksheet.Cells[i + 2, 3].Value = products[i].Description;
                    worksheet.Cells[i + 2, 4].Value = products[i].Price;
                    worksheet.Cells[i + 2, 5].Value = products[i].Image;
                }

                return package.GetAsByteArray();
            }
        }

    }
}
