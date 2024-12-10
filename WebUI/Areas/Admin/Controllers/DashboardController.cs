using Entities.Domains;
using Entities.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Services.PurchaseTransactionService.PurchaseTransactionVM;

namespace WebUI.Areas.Admin.Controllers
{
    [Area("admin")]
    [Authorize(Roles = "Admin")]
    public class DashboardController(IUnitOfWork unitOfWork) : Controller
    {
        private readonly IUnitOfWork unitOfWork = unitOfWork;

        public async Task<IActionResult> Index()
        {
            ViewBag.purchase = unitOfWork.GetRepository<PurchaseTransaction>()
                .GetAllAsync(x=>x.Status == "Pending")
                .GetAwaiter()
                .GetResult()
                .ToList()
                 .Count();
            ViewBag.loans = unitOfWork.GetRepository<BorrowinTransaction>()
                .GetAllAsync()
                .GetAwaiter()
                .GetResult()
                .ToList()
                 .Count();
            ViewBag.purchases = unitOfWork.GetRepository<PurchaseTransaction>()
                .GetAllAsync()
                .GetAwaiter()
                .GetResult()
                .ToList()
                 .Count();
            ViewBag.books = unitOfWork.GetRepository<Book>()
                .GetAllAsync()
                .GetAwaiter()
                .GetResult()
                .ToList()
                 .Count();
            int value = unitOfWork.GetRepository<User>().GetAllAsync()
                .GetAwaiter()
                .GetResult()
                .ToList()
                .Count();
            value--;
            ViewBag.users = value;

           

            return View();
        }
    }
}
