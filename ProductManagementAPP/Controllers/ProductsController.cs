using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProductManagementAPP.Services;
using ProductManagementAPP.Models;
using OfficeOpenXml;
using Microsoft.AspNetCore.Identity;
using ProductManagementAPP.ViewModels;

namespace ProductManagementAPP.Controllers
{
    public class ProductsController : Controller
    {
        private readonly IProductsService _productsService;
        private readonly ICategoriesService _categoriesService;
        private readonly UserManager<ApplicationUser> _userManager;
        public ProductsController(IProductsService productsService, ICategoriesService categoriesService , UserManager<ApplicationUser>userManager)
        {
            _productsService = productsService;
            _categoriesService = categoriesService;
            _userManager = userManager;
        }

        public async Task<IActionResult> Products(int pageNumber = 1, int pageSize = 10)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
                return Challenge();

            var userName = user.UserName;
            ViewData["UserName"] = userName;

            var products = await _productsService.GetAllProductsAsync(pageNumber, pageSize);
            return View(products);
        }

        public async Task<IActionResult> Create()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
                return Challenge();

            var userName = user.UserName;
            ViewData["UserName"] = userName;

            ViewBag.Categories = await _categoriesService.GetAllCategoriesAsync();
            return View(new ProductViewModel());
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([FromForm] ProductViewModel productViewModel, IFormFile image)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var user = await _userManager.GetUserAsync(User);
                    if (user == null)
                        return Challenge();

                    var userName = user.UserName;
                    ViewData["UserName"] = userName;

                    // Convert ProductViewModel to Product
                    var product = new Product
                    {
                        Name = productViewModel.Name,
                        Description = productViewModel.Description,
                        CategoryName = productViewModel.CategoryName,
                        Price = productViewModel.Price,
                        DateCreated = DateTime.Now,
                        CreatedBy = productViewModel.CreatedBy,
                        CategoryId = productViewModel.CategoryId
                    };

                    // Manually set the Category object
                    product.Category = await _categoriesService.GetCategoryByIdAsync(product.CategoryId);

                    if (image != null && image.Length > 0)
                    {
                        var fileName = Path.GetFileName(image.FileName);
                        var filePath = Path.Combine("wwwroot/images/uploads", fileName);
                        using (var fileStream = new FileStream(filePath, FileMode.Create))
                        {
                            await image.CopyToAsync(fileStream);
                        }
                        product.Image = fileName;
                    }

                    await _productsService.AddProductAsync(product);
                    return RedirectToAction("Products", "Products");
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError(string.Empty, $"An error occurred while creating the product: {ex.Message}");
                }
            }
            else
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors);
                foreach (var error in errors)
                {
                    ModelState.AddModelError(string.Empty, error.ErrorMessage);
                }
            }

            ViewBag.Categories = await _categoriesService.GetAllCategoriesAsync();
            return View(productViewModel);
        }

        public async Task<IActionResult> Edit(int id)
        {
            var product = await _productsService.GetProductByIdAsync(id);
            if (product == null)
            {
                return NotFound();
            }

            var productViewModel = new ProductViewModel
            {
                Name = product.Name,
                Description = product.Description,
                CategoryName = product.CategoryName,
                Price = product.Price,
                Image = product.Image,
                DateCreated = product.DateCreated,
                CreatedBy = product.CreatedBy,
                CategoryId = product.CategoryId
            };

            ViewBag.Categories = await _categoriesService.GetAllCategoriesAsync();
            return View(productViewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [FromForm] ProductViewModel productViewModel, IFormFile image)
        {
           

            if (ModelState.IsValid)
            {
                try
                {
                    var product = new Product
                    {
                        ProductId = id,
                        Name = productViewModel.Name,
                        Description = productViewModel.Description,
                        CategoryName = productViewModel.CategoryName,
                        Price = productViewModel.Price,
                        DateCreated = productViewModel.DateCreated,
                        CreatedBy = productViewModel.CreatedBy,
                        CategoryId = productViewModel.CategoryId
                    };

                    if (image != null && image.Length > 0)
                    {
                        var fileName = Path.GetFileName(image.FileName);
                        var filePath = Path.Combine("wwwroot/images/uploads", fileName);
                        using (var fileStream = new FileStream(filePath, FileMode.Create))
                        {
                            await image.CopyToAsync(fileStream);
                        }
                        product.Image = fileName;
                    }
                    else
                    {
                        product.Image = productViewModel.Image; 
                    }

                    await _productsService.UpdateProductAsync(product);
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError(string.Empty, $"An error occurred while updating the product: {ex.Message}");
                }
            }

            ViewBag.Categories = await _categoriesService.GetAllCategoriesAsync();
            return View(productViewModel);
        }

        public async Task<IActionResult> Delete(int id)
        {
            var product = await _productsService.GetProductByIdAsync(id);
            if (product == null)
            {
                return NotFound();
            }

            var productViewModel = new ProductViewModel
            {
                Name = product.Name,
                Description = product.Description,
                CategoryName = product.CategoryName,
                Price = product.Price,
                Image = product.Image,
                DateCreated = product.DateCreated,
                CreatedBy = product.CreatedBy,
                CategoryId = product.CategoryId
            };

            return View(productViewModel);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await _productsService.DeleteProductAsync(id);
            return RedirectToAction(nameof(Index));
        }
    

    [HttpPost]
        public async Task<IActionResult> UploadProducts(IFormFile file)
        {
            if (file != null && file.Length > 0)
            {
                using (var stream = new MemoryStream())
                {
                    await file.CopyToAsync(stream);
                    using (var package = new ExcelPackage(stream))
                    {
                        var worksheet = package.Workbook.Worksheets.FirstOrDefault();
                        if (worksheet != null)
                        {
                            for (int row = 2; worksheet.Cells[row, 1].Value != null; row++)
                            {
                                var product = new Product
                                {
                                    ProductCode = worksheet.Cells[row, 1].Text,
                                    Name = worksheet.Cells[row, 2].Text,
                                    Description = worksheet.Cells[row, 3].Text,
                                    CategoryId = int.Parse(worksheet.Cells[row, 4].Text),
                                    Price = decimal.Parse(worksheet.Cells[row, 5].Text)
                                };
                                await _productsService.AddProductAsync(product);
                            }
                        }
                    }
                }
            }

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> DownloadProducts()
        {
            var products = await _productsService.GetAllProductsAsync(1, int.MaxValue);

            using (var package = new ExcelPackage())
            {
                var worksheet = package.Workbook.Worksheets.Add("Products");

                worksheet.Cells[1, 1].Value = "Product Code";
                worksheet.Cells[1, 2].Value = "Name";
                worksheet.Cells[1, 3].Value = "Description";
                worksheet.Cells[1, 4].Value = "Category";
                worksheet.Cells[1, 5].Value = "Price";

                for (int i = 0; i < products.Count; i++)
                {
                    worksheet.Cells[i + 2, 1].Value = products[i].ProductCode;
                    worksheet.Cells[i + 2, 2].Value = products[i].Name;
                    worksheet.Cells[i + 2, 3].Value = products[i].Description;
                    worksheet.Cells[i + 2, 4].Value = products[i].Category.Name;
                    worksheet.Cells[i + 2, 5].Value = products[i].Price;
                }

                var fileName = "Products.xlsx";
                var contentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                var fileContents = package.GetAsByteArray();

                return File(fileContents, contentType, fileName);
            }
        }
    }
}
