using Entities.Domains;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Services.AuthorServices;
using Services.AuthorServices.AuthorViewModels;
using WebUI.Commponents;
using X.PagedList.Extensions;

namespace WebUI.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = RoleConst.AdminRole)]

    public class AuthorController(IAuthorService authorService) : Controller
    {
        private readonly IAuthorService _authorService = authorService;

        public async Task<IActionResult> Index(int? page)
        {
            var pageNumber = page ?? 1;
            var pageSize = 8;
            var authors =  _authorService.GetAllAsync().GetAwaiter()
                .GetResult().ToPagedList(pageNumber, pageSize);
            return View(authors);
        }

        public async Task<IActionResult> Create()
        {
            return View();
        }

        [HttpPost]
        [AutoValidateAntiforgeryToken]
        public async Task<IActionResult> Create(AuthorCreateVM author)
        {
            if (ModelState.IsValid)
            {
                await _authorService.AddAsync(author);
                return RedirectToAction("Index");
            }
            return View(author);
        }

        public async Task<IActionResult> Edit(int id)
        {
            var author = await _authorService.GetByIdAsync(id);
            return View(author);
        }

        [HttpPost]
        [AutoValidateAntiforgeryToken]
        public async Task<IActionResult> Edit(AuthorUpdateVM author)
        {
            if (ModelState.IsValid)
            {
                await _authorService.UpdateAsync(author);
                return RedirectToAction("Index");
            }
            return View(author);
        }

        public async Task<IActionResult> Delete(int id)
        {
            var author = await _authorService.GetByIdAsync(id);
            return View(author);
        }

        [HttpPost]
        [AutoValidateAntiforgeryToken]
        public async Task<IActionResult> Delete(AuthorVM author)
        {
            await _authorService.DeleteAsync(author);
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> Details(int id)
        {
            var author = await _authorService.GetByIdAsync(id);
            return View(author);
        }
    }
}
