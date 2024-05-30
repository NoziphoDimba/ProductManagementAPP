﻿using Microsoft.AspNetCore.Mvc;

namespace ProductManagementAPP.Controllers
{
    public class CategoriesViewController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Create()
        {
            return View();
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
