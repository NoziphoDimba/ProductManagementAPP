using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using ProductManagementAPP.Models;
using ProductManagementAPP.Services;

namespace ProductManagementAPP.Controllers
{
    public class CategoriesViewController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        public CategoriesViewController( UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }
        public IActionResult AllCategories()
        {
            return View();
        }

        [HttpGet("Create")]
        public async Task<IActionResult> CreateCategory()
        {
            var user = await _userManager.GetUserAsync(User);
            var userName = user?.UserName ?? "Guest";
            return Ok(new { UserName = userName });
        }

        public IActionResult Edit(int id)
        {
            ViewBag.CategoryId = id;
            return View();
        }

        public IActionResult Delete(int id)
        {
            ViewBag.CategoryId = id;
            return View();
        }
    }
}
