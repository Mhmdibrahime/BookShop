using DataAccess.Implementation;
using Entities.Domains;
using Entities.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Services.ShoppingCartServices.ShopingCartViewModels;
using Services.ShoppingCartServices;
using System.Security.Claims;
using Stripe;

namespace WebUI.Areas.Identity.Pages.Account.Manage
{
    public class BorrowingModel : PageModel
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICartService cartService;

        public BorrowingModel(IUnitOfWork unitOfWork, ICartService cartService)
        {
            _unitOfWork = unitOfWork;
            this.cartService = cartService;
        }

        public IEnumerable<BorrowinTransaction> borrowinTransactions = [];
        public async void OnGet()
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity!;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
            var borrowines = _unitOfWork.GetRepository<BorrowinTransaction>()
                .GetAllAsync(s=>s.UserId == claim!.Value && s.Status != "Returned" && s.Status != "Pending", b => b.Book)
                .GetAwaiter()
                .GetResult();
            borrowinTransactions = borrowines.ToList();
        }

        public async Task<IActionResult> OnPostRenewAsync(int id)
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity!;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
            var book = (await _unitOfWork.GetRepository<BorrowinTransaction>()
                .GetAllAsync(null, b => b.Book, c => c.Book.Category))
                .FirstOrDefault(x => x.BookId == id && x.UserId == claim.Value);

            if (book != null)
            {
                if (book.FineAmount > 0)
                {
                    TempData["ErrorMessage"] = "You cannot renew this book as there is a fine amount pending.";
                    return RedirectToPage("Borrowing");
                }
                if (book.Status == "Rent")
                {
                    TempData["ErrorMessage"] = "This book is currently rented and cannot be renewed.";
                    return RedirectToPage("Borrowing");
                }
                if (book.ReturnDate < DateTime.Now)
                {
                    TempData["ErrorMessage"] = "This book is overdue and cannot be renewed.";
                    return RedirectToPage("Borrowing");
                }
                if (book.Book.Stock == 0)
                {
                    TempData["ErrorMessage"] = "This book is not available for renewal.";
                    return RedirectToPage("Borrowing");
                }

                try
                {
                    var shoppingCartItem = new CartVM
                    {
                        ApplicationUserId = claim.Value,
                        ProductId = book.BookId,
                        Count = 1,
                        IsBorrowed = true,
                    };
                    var existingCartItem = (await _unitOfWork.GetRepository<ShopingCart>()
                                       .GetAllAsync())
                                       .FirstOrDefault(x => x.ApplicationUserId == claim.Value && x.BookId == shoppingCartItem.ProductId && x.IsBorrowed ==true);
                    if (existingCartItem != null) 
                    {
                        existingCartItem.Count += shoppingCartItem.Count;
                        _unitOfWork.GetRepository<ShopingCart>().UpdateAsync(existingCartItem).GetAwaiter().GetResult();
                         _unitOfWork.SaveChangesAsync().GetAwaiter().GetResult();
                        return Redirect("/Customer/Home/Cart");
                    }

                    await cartService.AddCartAsync(shoppingCartItem);

                    return Redirect("/Customer/Home/Cart");
                }
                catch
                {
                    return RedirectToPage("Borrowing");
                }
            }
            else
            {
                return RedirectToPage("Borrowing");

            }
        }
        

    }



}

