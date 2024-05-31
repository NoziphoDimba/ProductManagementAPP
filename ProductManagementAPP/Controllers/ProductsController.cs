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
using Microsoft.Extensions.Configuration.UserSecrets;

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

            var userId = user.Id; 
            var userName = user.UserName;
            ViewData["UserName"] = userName;

            var products = await _productsService.GetAllProductsAsync(pageNumber, pageSize, userId);
            return View(products);
        }

        public async Task<IActionResult> Create()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
                return Challenge();
            var userId = user.Id;
            var userName = user.UserName;
            ViewData["UserName"] = userName;

            ViewBag.Categories = await _categoriesService.GetAllCategoriesAsync(userId);
            return View(new ProductViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([FromForm] ProductViewModel productViewModel, IFormFile? image)
        {
            var user = await _userManager.GetUserAsync(User);
            var userId = user.Id;
            if (ModelState.IsValid)
            {
                try
                {
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
                        UserId = userId,
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

            ViewBag.Categories = await _categoriesService.GetAllCategoriesAsync(userId);
            return View(productViewModel);
        }
       
        public async Task<IActionResult> GetProduct(int id)
        {
            var product = await _productsService.GetProductByIdAsync(id);
            if (product == null)
            {
                return NotFound();
            }
            return Json(product);
        }
        [HttpGet]
        public async Task<IActionResult> GetAllCategories()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
                return Challenge();

            var userId = user.Id;
         
            var categories = await _categoriesService.GetAllCategoriesAsync(userId);
            return Json(categories);
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

        [HttpPost]
        public async Task<IActionResult> Edit([FromForm] ProductViewModel productViewModel)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var user = await _userManager.GetUserAsync(User);
                    if (user == null)
                        return Challenge();
                    var userId = user.Id;
                    var product = await _productsService.GetProductByIdAsync(productViewModel.ProductId);
                    if (product == null)
                    {
                        return NotFound();
                    }
                   
                   product.DateCreated = DateTime.Now;
                    product.UserId = userId;

                    await _productsService.UpdateProductAsync(product);

                    return Json(new { success = true, message = "Product updated successfully." });
                }
                catch (Exception ex)
                {
                    return Json(new { success = false, message = $"An error occurred while updating the product: {ex.Message}" });
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
            return Json(new { success = false, message = "Invalid model state. Please check your inputs." });
        }


        [HttpPost]
        public async Task<IActionResult> Delete(int productId)
        {
            try
            {
                await _productsService.DeleteProductAsync(productId);
                return Json(new { success = true, message = "Product deleted successfully." });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = $"An error occurred while deleting the product: {ex.Message}" });
            }
        }

    }
}
