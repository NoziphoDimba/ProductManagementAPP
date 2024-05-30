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
        public async Task<IActionResult> Create([FromForm] ProductViewModel productViewModel, IFormFile? image)
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

                  
                    var product = new Product
                    {
                        Name = productViewModel.Name,
                        Description = productViewModel.Description,
                        CategoryName = productViewModel.CategoryName,
                        Price = productViewModel.Price,
                        DateCreated = DateTime.Now,
                        UserId = user.Id,
                        CreatedBy = productViewModel.CreatedBy,
                        CategoryId = productViewModel.CategoryId
                    };

                   
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
                   
                    return Json(new { success = true, message = "Success" });
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

        public async Task<IActionResult> GetCategory(int id)
        {
            var category = await _categoriesService.GetCategoryByIdAsync(id);
            if (category == null)
            {
                return NotFound();
            }

            var categoryViewModel = new CategoryViewModel
            {
                CategoryId = category.CategoryId,
                Name = category.Name,
                CategoryCode = category.CategoryCode,
                IsActive = category.IsActive
            };

            return Json(categoryViewModel);
        }

        [HttpPost]
        public async Task<IActionResult> UploadExcel(IFormFile excelFile, int productId)
        {
            if (excelFile == null || excelFile.Length == 0)
            {
                return BadRequest("Invalid file");
            }

            using (var stream = new MemoryStream())
            {
                await excelFile.CopyToAsync(stream);
                var result = await _productsService.ProcessExcelFileAsync(stream, productId);
                if (result)
                {
                    TempData["SuccessMessage"] = "Products uploaded successfully.";
                    return Ok();
                }
                else
                {
                    return StatusCode(500, "An error occurred while processing the file.");
                }
            }
        }

        public async Task<IActionResult> DownloadExcel(int productId)
        {
            var fileContent = await _productsService.GenerateExcelFileAsync(productId);
            if (fileContent == null || fileContent.Length == 0)
            {
                return NotFound();
            }

            return File(fileContent, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "products.xlsx");
        }

    }
}
