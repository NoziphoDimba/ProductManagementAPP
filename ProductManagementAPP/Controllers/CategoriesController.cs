//using System.Threading.Tasks;
//using Microsoft.AspNetCore.Identity;
//using Microsoft.AspNetCore.Mvc;
//using ProductManagementAPP.Models;
//using ProductManagementAPP.Services;

//namespace ProductManagementAPP.Controllers
//{
//    public class CategoriesController : Controller
//    {
//        private readonly ICategoriesService _categoriesService;
//        private readonly UserManager<ApplicationUser> _userManager;

//        public CategoriesController(ICategoriesService categoriesService, UserManager<ApplicationUser> userManager)
//        {
//            _categoriesService = categoriesService;
//            _userManager = userManager;
//        }

//        public async Task<IActionResult> Categories()
//        {
//            var user = await _userManager.GetUserAsync(User);
//            if (user == null)
//                return Challenge();

//            var userName = user.UserName;
//            ViewData["UserName"] = userName;

//            var categories = await _categoriesService.GetAllCategoriesAsync();
//            return View(categories);
//        }

//        public async Task< IActionResult> Create()
//        {
//            var user = await _userManager.GetUserAsync(User);
//            var userName = user?.UserName ?? "Guest";
//            ViewData["UserName"] = userName;
//            return View();
//        }

//        [HttpPost]
//        [ValidateAntiForgeryToken]
//        public async Task<IActionResult> Create(Category category)
//        {
//            var user = await _userManager.GetUserAsync(User);
//            var userName = user?.UserName ?? "Guest";
//            if (ModelState.IsValid)
//            {
//                try
//                {
//                    await _categoriesService.AddCategoryAsync(category, userName);
//                    TempData["SuccessMessage"] = "Category added successfully.";
//                    return RedirectToAction("Categories", "Categories");
//                }
//                catch (Exception ex)
//                {
//                    ModelState.AddModelError(string.Empty, $"An error occurred while creating the category: {ex.Message}");
//                }
//            }
//            else
//            {
//                var errors = ModelState.Values.SelectMany(v => v.Errors);
//                foreach (var error in errors)
//                {
//                    ModelState.AddModelError(string.Empty, error.ErrorMessage);
//                }
//            }
//            return View(category);
//        }

//        public async Task<IActionResult> Edit(int id)
//        {
//            var category = await _categoriesService.GetCategoryByIdAsync(id);
//            if (category == null)
//            {
//                return NotFound();
//            }
//            return View(category);
//        }

//        [HttpPost]
//        [ValidateAntiForgeryToken]
//        public async Task<IActionResult> Edit(int id, Category category)
//        {
//            if (id != category.CategoryId)
//            {
//                return BadRequest();
//            }

//            if (ModelState.IsValid)
//            {
//                await _categoriesService.UpdateCategoryAsync(category);
//                return RedirectToAction(nameof(Index));
//            }
//            return View(category);
//        }

//        public async Task<IActionResult> Delete(int id)
//        {
//            var category = await _categoriesService.GetCategoryByIdAsync(id);
//            if (category == null)
//            {
//                return NotFound();
//            }
//            return View(category);
//        }

//        [HttpPost, ActionName("Delete")]
//        [ValidateAntiForgeryToken]
//        public async Task<IActionResult> DeleteConfirmed(int id)
//        {
//            await _categoriesService.DeleteCategoryAsync(id);
//            return RedirectToAction(nameof(Index));
//        }
//    }
//}

using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using ProductManagementAPP.Models;
using ProductManagementAPP.Services;

namespace ProductManagementAPP.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoriesController : ControllerBase
    {
        private readonly ICategoriesService _categoriesService;
        private readonly UserManager<ApplicationUser> _userManager;

        public CategoriesController(ICategoriesService categoriesService, UserManager<ApplicationUser> userManager)
        {
            _categoriesService = categoriesService;
            _userManager = userManager;
        }

        [HttpGet]
        public async Task<IActionResult> Categories()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
                return Unauthorized();

            var categories = await _categoriesService.GetAllCategoriesAsync();
            return Ok(categories);
        }

        [HttpGet("Create")]
        public async Task<IActionResult> CreateCategory()
        {
            var user = await _userManager.GetUserAsync(User);
            var userName = user?.UserName ?? "Guest";
            return Ok(new { UserName = userName });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateCategory([FromBody] Category category)
        {
            var user = await _userManager.GetUserAsync(User);
            var userName = user?.UserName ?? "Guest";
            if (ModelState.IsValid)
            {
                try
                {
                    await _categoriesService.AddCategoryAsync(category, userName);
                    return Ok(new { Message = "Category added successfully." });
                }
                catch (Exception ex)
                {
                    return BadRequest(new { Error = $"An error occurred while creating the category: {ex.Message}" });
                }
            }
            return BadRequest(ModelState);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetCategory(int id)
        {
            var category = await _categoriesService.GetCategoryByIdAsync(id);
            if (category == null)
            {
                return NotFound();
            }
            return Ok(category);
        }

        [HttpPut("{id}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditCategory(int id, [FromBody] Category category)
        {
            if (id != category.CategoryId)
            {
                return BadRequest();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    await _categoriesService.UpdateCategoryAsync(category);
                    return Ok(new { Message = "Category updated successfully." });
                }
                catch (Exception ex)
                {
                    return BadRequest(new { Error = $"An error occurred while updating the category: {ex.Message}" });
                }
            }
            return BadRequest(ModelState);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCategory(int id)
        {
            var category = await _categoriesService.GetCategoryByIdAsync(id);
            if (category == null)
            {
                return NotFound();
            }

            return Ok(category);
        }

        [HttpPost("Delete/{id}")]
        [ActionName("DeleteCategory")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteCategoryConfirmed(int id)
        {
            await _categoriesService.DeleteCategoryAsync(id);
            return Ok(new { Message = "Category deleted successfully." });
        }
    }
}
