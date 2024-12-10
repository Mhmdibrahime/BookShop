using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Services.CategoryServices;
using Services.CategoryServices.CategoryViewModels;
using WebUI.Commponents;

namespace WebUI.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = RoleConst.AdminRole)]

    public class CategoryController(ICategoryService categoryService) : Controller
    {
        private readonly ICategoryService _categoryService = categoryService;

        public async Task<IActionResult> Index()
        {
            var categories = await _categoryService.GetAllAsync();

            return View(categories);
        }
        [HttpGet]
        public async Task<IActionResult> Create()
        {
            return View();
        }

        [HttpPost]
        [AutoValidateAntiforgeryToken]
        public async Task<IActionResult> Create(CategoryCreateVM category)
        {
            if (ModelState.IsValid)
            {
                await _categoryService.AddAsync(category);
                return RedirectToAction("Index");
            }
            return View(category);
        }

        public async Task<IActionResult> Edit(int id)
        {
            var category = await _categoryService.GetByIdAsync(id);
            return View(category);
        }

        [HttpPost]
        [AutoValidateAntiforgeryToken]
        public async Task<IActionResult> Edit(CategoryUpdateVM category)
        {
            if (ModelState.IsValid)
            {
                await _categoryService.UpdateAsync(category);
                return RedirectToAction("Index");
            }
            return View(category);
        }

        public async Task<IActionResult> Delete(int id)
        {
            var category = await _categoryService.GetByIdAsync(id);
            return View(category);
        }

        [HttpPost]
        [AutoValidateAntiforgeryToken]
        public async Task<IActionResult> Delete(CategoryUpdateVM category)
        {
            await _categoryService.DeleteAsync(category);
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> Details(int id)
        {
            var category = await _categoryService.GetByIdAsync(id);
            return View(category);
        }
    }
}
