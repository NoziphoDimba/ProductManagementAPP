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

            var userName = user.UserName;
            ViewData["UserName"] = userName;

            var categories = await _categoriesService.GetAllCategoriesAsync();
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
                    TempData["SuccessMessage"] = "Category added successfully.";
                    return Json(new { success = true, message = "Category added successfully." });
                }
                catch (Exception ex)
                {
                    return Json(new { success = false, message = $"An error occurred while creating the category: {ex.Message}" });
                }
            }

            return Json(new { success = false, message = "Model state is invalid." });
        }

        public async Task<IActionResult> Edit(int id)
        {
            var category = await _categoriesService.GetCategoryByIdAsync(id);
            if (category == null)
            {
                return NotFound();
            }
            return View(category);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Category category)
        {
            if (id != category.CategoryId)
            {
                return BadRequest();
            }

            if (ModelState.IsValid)
            {
                await _categoriesService.UpdateCategoryAsync(category);
                return RedirectToAction(nameof(Index));
            }
            return View(category);
        }

        public async Task<IActionResult> Delete(int id)
        {
            var category = await _categoriesService.GetCategoryByIdAsync(id);
            if (category == null)
            {
                return NotFound();
            }
            return View(category);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await _categoriesService.DeleteCategoryAsync(id);
            return RedirectToAction(nameof(Index));
        }
    }
}

