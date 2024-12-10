using Entities.Domains;
using Entities.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Services.PublisherServices;
using Services.PublisherServices.PublisherViewModels;
using WebUI.Commponents;

namespace WebUI.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = RoleConst.AdminRole)]

    public class PublisherController(IPublisherService PublisherService, IUnitOfWork unitOfWork) : Controller
    {
        private readonly IPublisherService _PublisherService = PublisherService;
        private readonly IUnitOfWork _unitOfWork = unitOfWork;

        public async Task<IActionResult> Index()
        {
            return View();
        }
        [HttpGet]
        public async Task<IActionResult> GetAllPublishers()
        {

            var Publishers = await _PublisherService.GetAllAsync(x=>x.IsDeleted != true);
            return Json(new { data = Publishers });
        }

        public async Task<IActionResult> Create()
        {
            return View();
        }

        [HttpPost]
        [AutoValidateAntiforgeryToken]
        public async Task<IActionResult> Create(PublisherCreateVM Publisher)
        {
            if (ModelState.IsValid)
            {
                await _PublisherService.AddAsync(Publisher);
                return RedirectToAction("Index");
            }
            return View(Publisher);
        }
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var Publisher = await _PublisherService.GetByIdAsync(id);
            return View(Publisher);
        }

        [HttpPost]
        [AutoValidateAntiforgeryToken]
        public async Task<IActionResult> Edit(PublisherUpdateVM Publisher)
        {
            if (ModelState.IsValid)
            {
                await _PublisherService.UpdateAsync(Publisher);
                return RedirectToAction("Index");
            }
            return View(Publisher);
        }

        [HttpDelete]
        public async Task<IActionResult> Delete(int id)
        {
            if (id <= 0)
            {
                return Json(new { success = false, message = "Invalid Publisher ID" });
            }

            var publisher = await _unitOfWork.GetRepository<Publisher>().GetByIdAsync(id);

            if (publisher == null)
            {
                return Json(new { success = false, message = "Publisher not found" });
            }

            publisher.IsDeleted = true;
            await _unitOfWork.SaveChangesAsync();
            
            return Json(new { success = true, message = "Publisher has been deleted" });
        }

        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            var Publisher = await _PublisherService.GetByIdAsync(id);
            return View(Publisher);
        }
    }
}
