using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using ProductManagementAPP.Models;
using ProductManagementAPP.Services;
using ProductManagementAPP.ViewModels;
using System.Diagnostics;

namespace ProductManagementAPP.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IProductsService _productService;
        private readonly ICategoriesService _categoryService;
        public HomeController(ILogger<HomeController> logger, SignInManager<ApplicationUser> signInManager, UserManager<ApplicationUser> userManager , IProductsService productsService , ICategoriesService categoriesService)
        {
            _logger = logger;
            _signInManager = signInManager;
            _userManager = userManager;
            _categoryService = categoriesService;
            _productService = productsService;
        }

        public async Task<IActionResult> IndexAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
                return Challenge();

            var userName = user.UserName;
            var userId = user.Id;
            ViewData["UserName"] = userName;

            var totalCategories = await _categoryService.CountCategories(userId);
            var totalProducts = await _productService.CountProducts(userId);

            var viewModel = new DashboardViewModel
            {
                TotalCategories = totalCategories,
                TotalProducts = totalProducts
            };

            return View(viewModel);

        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return LocalRedirect("/Identity/Account/Login");
        }
    }
}