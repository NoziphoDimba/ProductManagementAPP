using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using ProductManagementAPP.Models;
using ProductManagementAPP.Services;
using ProductManagementAPP.ViewModels;

namespace ProductManagementAPP.Controllers
{
    public class CategoriesController : Controller
    {
        private readonly ICategoriesService _categoriesService;
        private readonly UserManager<ApplicationUser> _userManager;

        public CategoriesController(ICategoriesService categoriesService, UserManager<ApplicationUser> userManager)
        {
            _categoriesService = categoriesService;
            _userManager = userManager;
        }

        public async Task<IActionResult> Categories()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
                return Challenge();

            var userId = user.Id;
            var userName = user.UserName;
            ViewData["UserName"] = userName;

            var categories = await _categoriesService.GetAllCategoriesAsync(userId);
            return View(categories);
        }


        public async Task<IActionResult> Create()
        {
            var user = await _userManager.GetUserAsync(User);
            var userName = user?.UserName ?? "Guest";
            ViewData["UserName"] = userName;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CategoryViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var user = await _userManager.GetUserAsync(User);
                    var userId = user?.Id;
                    var userName = user?.UserName ?? "Guest";

                    var category = new Category
                    {
                        Name = viewModel.Name,
                        CategoryCode = viewModel.CategoryCode,
                        IsActive = viewModel.IsActive,
                        CreatedBy = userName
                    };

                    await _categoriesService.AddCategoryAsync(category, userId, userName);
                   
                    return Json(new { success = true, message = "Category added successfully." });
                }
                catch (Exception ex)
                {
                    return Json(new { success = false, message = $"An error occurred while creating the category: {ex.Message}" });
                }
            }

            return Json(new { success = false, message = "Model state is invalid." });
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var category = await _categoriesService.GetCategoryByIdAsync(id);
            if (category == null)
            {
                return NotFound();
            }
            var viewModel = new CategoryViewModel
            {
                Name = category.Name,
                CategoryCode = category.CategoryCode,
                IsActive = category.IsActive,
                
            };
            return View(viewModel);
        }
        [HttpPost]
        public async Task<IActionResult> Edit(CategoryViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var user = await _userManager.GetUserAsync(User);
                    var userId = user?.Id;
                    var userName = user?.UserName ?? "Guest";

                    var category = await _categoriesService.GetCategoryByIdAsync(viewModel.CategoryId);
                    if (category == null)
                    {
                        return NotFound();
                    }

                    category.Name = viewModel.Name;
                    category.CategoryCode = viewModel.CategoryCode;
                    category.IsActive = viewModel.IsActive;
                    category.DateCreated = DateTime.Now;
                    category.CreatedBy = userId;

                    await _categoriesService.UpdateCategoryAsync(category);
                    return Json(new { success = true });
                }
                catch (Exception ex)
                {
                    return Json(new { success = false, message = ex.Message });
                }
            }

            return Json(new { success = false, message = "Invalid data" });
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                await _categoriesService.DeleteCategoryAsync(id);
                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
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


    }
}

