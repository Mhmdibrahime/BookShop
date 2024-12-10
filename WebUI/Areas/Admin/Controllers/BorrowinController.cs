using DataAccess.Implementation;
using Entities.Domains;
using Entities.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NuGet.Protocol.Core.Types;
using Services.BorrowinTransactionService.BorrowinTransactionVM;
using Services.PurchaseTransactionService.PurchaseTransactionVM;
using System.Web.Helpers;
using WebUI.Commponents;
using X.PagedList.Extensions;

namespace WebUI.Areas.Admin.Controllers
{
    [Area("admin")]
    [Authorize(Roles = RoleConst.AdminRole)]
    public class BorrowinController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IEmailSender emailSender;

        public BorrowinController(IUnitOfWork unitOfWork, IEmailSender emailSender)
        {
            this._unitOfWork = unitOfWork;
            this.emailSender = emailSender;
        }

        public async Task<IActionResult> Index(int? page, string? statusFilter)
        {
            var pageNumber = page ?? 1;
            var pageSize = 8;


            var purchases = await _unitOfWork.GetRepository<BorrowinTransaction>()
              .GetAllAsync(null, u => u.User, b => b.Book);





            if (!string.IsNullOrEmpty(statusFilter))
            {
                if (statusFilter == "Expired")
                {
                    purchases = purchases.Where(t => t.Status == "Expired");
                }
                else if (statusFilter == "WithFineReturned")
                {
                    purchases = purchases.Where(t => t.FineAmount > 0 && t.Status == "Returned");
                }
                else if (statusFilter == "WithFine")
                {
                    purchases = purchases.Where(t => t.FineAmount > 0 && t.Status == "Fine");
                }
                else if (statusFilter == "UpcomingReturn")
                {
                    purchases = purchases.Where(t => t.ReturnDate <= DateTime.Now.AddDays(3) && t.ReturnDate > DateTime.Now);
                }
                else if(statusFilter == "Pending")
                {
                    purchases = purchases.Where(t => t.Status == "Pending" );

                }
                else if (statusFilter == "All")
                {
                    return View(purchases.ToPagedList(pageNumber, pageSize));
                }


            }

            return View(purchases.ToPagedList(pageNumber, pageSize));

        }
        [HttpPost]

        public async Task<IActionResult> SendReminder(int transactionId)
        {
            var transaction = (await _unitOfWork.GetRepository<BorrowinTransaction>()
                   .GetAllAsync(x => x.Id == transactionId, u => u.User, b => b.Book))
                   .FirstOrDefault();

            if (transaction == null)
            {
                return Json(new { success = false, message = "Transaction not found." });
            }

            if (transaction.User == null || transaction.Book == null)
            {
                return Json(new { success = false, message = "User or Book information is missing." });
            }

            var message = $"Dear {transaction.User.FullName},\n\n" +
                          $"This is a reminder that you have a book '{transaction.Book.Title}' due for return on {transaction.ReturnDate:yyyy-MM-dd}. " +
                          "Please make sure to return it on time to avoid any fines.\n\nThank you!";

            try
            {
                await emailSender.SendEmailAsync(transaction.User.Email!, transaction.User.FullName, message);

                var notification = new Notification
                {
                    CreatedAt = DateTime.Now,
                    DueDate = transaction.ReturnDate,
                    IsRead = false,
                    Message = transaction.FineAmount > 0 || transaction.Status == "Fine"
                        ? $"Your book was due on {transaction.ReturnDate:yyyy-MM-dd}.\nA fine of {transaction.FineAmount:C} has been imposed.\nPlease return it as soon as possible."
                        : $"This is a reminder that you have a book due for return on {transaction.ReturnDate:yyyy-MM-dd}.",
                    UserId = transaction.User.Id,
                    BookName = transaction.Book.Title!
                };

                await _unitOfWork.GetRepository<Notification>().AddAsync(notification);
                await _unitOfWork.SaveChangesAsync();

                return Json(new { success = true, message = "The message was sent successfully" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "An error occurred while sending the email." });
            }

        }

        [HttpPost]
        public async Task<IActionResult> SendBulkReminder()
        {
            try
            {
                var transactions = await _unitOfWork.GetRepository<BorrowinTransaction>()
                    .GetAllAsync(t => t.ReturnDate <= DateTime.Now.AddDays(3) && t.ReturnDate > DateTime.Now);

                var reminderTasks = transactions.Select(transaction => SendReminder(transaction.Id)).ToList();

                await Task.WhenAll(reminderTasks);

                return Json(new { success = true, message = "Reminders sent successfully to all relevant borrowers." });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = $"Failed to send reminders. Error: {ex.Message}" });
            }
        }


        public async Task<IActionResult> Details(int? id)
        {

            var purchases = _unitOfWork.GetRepository<BorrowinTransaction>()
                .GetAllAsync(x => x.Id == id, u => u.User, b => b.Book, c => c.Book.Category)
                .GetAwaiter()
                .GetResult()
                .FirstOrDefault();



            return View(purchases);
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int? id)
        {
            var bookBorrowin = _unitOfWork.GetRepository<BorrowinTransaction>()
                .GetAllAsync(x => x.Id == id, u => u.User, b => b.Book, c => c.Book.Category)
                .GetAwaiter()
                .GetResult()
                .FirstOrDefault();
            return View(bookBorrowin);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(BorrowinTransaction? model)
        {

            var bookBorrowin = _unitOfWork.GetRepository<BorrowinTransaction>()
                .GetAllAsync(x => x.Id == model!.Id, u => u.User, b => b.Book, c => c.Book.Category)
                .GetAwaiter()
                .GetResult()
                .FirstOrDefault();

            if (bookBorrowin != null)
            {
                bookBorrowin.ReturnDate = model!.ReturnDate;
                bookBorrowin.BorrowDate = model.BorrowDate;
                bookBorrowin.FineAmount = model.FineAmount;
                bookBorrowin.Status = model.Status;
               
                await _unitOfWork.GetRepository<BorrowinTransaction>()
                .UpdateAsync(bookBorrowin);
                await _unitOfWork.SaveChangesAsync();

                return Redirect($"/Admin/Borrowin/Details/{model.Id}");
            }


            return View(bookBorrowin);
        }

        public async Task<IActionResult> Expired(int? id)
        {
            var bookBorrowin = await _unitOfWork.GetRepository<BorrowinTransaction>()
                            .GetByIdAsync(id!);

            if (bookBorrowin != null)
            {
                bookBorrowin.Status = "Expired";
                await _unitOfWork.GetRepository<BorrowinTransaction>()
                .UpdateAsync(bookBorrowin!);

                await _unitOfWork.SaveChangesAsync();

                return RedirectToAction("Details", new { id = id });
            }

            return RedirectToAction("Index");
        }
        public async Task<IActionResult> Return(int? id)
        {
            var bookBorrowin = await _unitOfWork.GetRepository<BorrowinTransaction>()
                .GetByIdAsync(id!);

            if (bookBorrowin != null)
            {
                bookBorrowin.Status = "Returned";
                await _unitOfWork.GetRepository<BorrowinTransaction>()
                .UpdateAsync(bookBorrowin!);

                await _unitOfWork.SaveChangesAsync();

                return RedirectToAction("Details", new { id = id });
            }

            return RedirectToAction("Index");

        }

        public async Task<IActionResult> Delete(int? id)
        {
            try
            {
                var bookBorrowin = _unitOfWork.GetRepository<BorrowinTransaction>()
                .GetByIdAsync(id!)
                .GetAwaiter()
                .GetResult();

                if (bookBorrowin == null)
                {
                    return Json(new { success = false, message = "Borrowin not found" });
                }


                _unitOfWork.GetRepository<BorrowinTransaction>()
                 .DeleteAsync(bookBorrowin!)
                 .GetAwaiter()
               .GetResult();

                _unitOfWork.SaveChangesAsync()
                   .GetAwaiter()
               .GetResult();
            }
            catch
            {
                return Json(new { success = false, message = "Borrowin not found" });

            }

            return Json(new { success = true, message = "Borrowin has been deleted" });


        }

    }

}
