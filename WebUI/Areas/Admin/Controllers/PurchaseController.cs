using Entities.Domains;
using Entities.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Services.PurchaseTransactionService.PurchaseTransactionVM;
using Stripe;
using WebUI.Commponents;
using X.PagedList.Extensions;

namespace WebUI.Areas.Admin.Controllers
{
    [Area("admin")]
    [Authorize(Roles = RoleConst.AdminRole)]
    public class PurchaseController(IUnitOfWork unitOfWork) : Controller
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;

        public async Task<IActionResult> Index(int? page)
        {
            var pageNumber = page ?? 1;
            var pageSize = 8;
            var purchases = _unitOfWork.GetRepository<PurchaseTransaction>()
                .GetAllAsync(null, u => u.User, b => b.Book)
                .GetAwaiter()
                .GetResult()
                .Select(p => new PurchaseVMV
                {
                    Id = p.Id,
                    UserName = p.User.FullName,
                    Address = p.User.Address!,
                    Phone = p.User.PhoneNumber!,
                    Email = p.User.Email!,
                    BookTitle = p.Book.Title!,
                    BookPrice = p.Price,
                    PurchaseCount = p.Quantity,
                    TotalPrice = p.TotalAmount,
                    PurchaseDate = p.PurchaseDate,
                    Status = p.Status?? "Pending"
                }).ToPagedList(pageNumber, pageSize);
           


            return View(purchases);
        }
        public async Task<IActionResult> Details(int? id)
        {
            var purchase = _unitOfWork.GetRepository<PurchaseTransaction>()
                .GetAllAsync(null, u => u.User, b => b.Book)
                .GetAwaiter()
                .GetResult()
                .FirstOrDefault(x => x.Id == id);

            var purchaseVM = new PurchaseVMV
            {
                Id = purchase.Id,
                UserName = purchase.User.FullName!,
                Address = purchase.User.Address!,
                Phone = purchase.User.PhoneNumber!,
                Email = purchase.User.Email!,
                BookTitle = purchase.Book.Title!,
                BookPrice = purchase.Price,
                PurchaseCount = purchase.Quantity,
                TotalPrice = purchase.TotalAmount,
                PurchaseDate = purchase.PurchaseDate,
                Status = purchase.Status ?? "Pending"
            };
            

            var purchaseVMD = new PurchaseVMD
            {
                PurchaseVMV = purchaseVM,
            };
            return View(purchaseVMD);
        }
       
        public async Task<IActionResult> StartProcs(int? id)
        {

            var purchase =  await _unitOfWork.GetRepository<PurchaseTransaction>().GetByIdAsync(id);
            purchase.Status = "Processing";
            await _unitOfWork.GetRepository<PurchaseTransaction>().UpdateAsync(purchase);
            await _unitOfWork.SaveChangesAsync();
            TempData["purchaseProcess"] = "ok";
            return Redirect($"/Admin/Purchase/Details/{id}");
        }

      
        public async Task<IActionResult> StartShip(int? id)
        {
            var purchase = await _unitOfWork.GetRepository<PurchaseTransaction>().GetByIdAsync(id);
            purchase.Status = "Complete"; 
            await _unitOfWork.GetRepository<PurchaseTransaction>().UpdateAsync(purchase);
            await _unitOfWork.SaveChangesAsync();

            TempData["purchaseShip"] = "ok";
            return Redirect($"/Admin/Purchase/Details/{id}");
        }

     
        public async Task<IActionResult> CancelOrder(int? id)
        {
            var purchase = await _unitOfWork.GetRepository<PurchaseTransaction>().GetByIdAsync(id);

            purchase.Status = "Cancelled";
            await _unitOfWork.GetRepository<PurchaseTransaction>().UpdateAsync(purchase);
            await _unitOfWork.SaveChangesAsync();

            TempData["OrderCancel"] = "ok";
            return Redirect($"/Admin/Purchase/Details/{id}");
        }
    }
}
