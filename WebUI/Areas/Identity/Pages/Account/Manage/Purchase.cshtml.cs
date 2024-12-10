using Entities.Domains;
using Entities.Repositories;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Services.PurchaseTransactionService.PurchaseTransactionVM;
using Services.RateService;
using Services.RateService.RateServiceVM;
using Services.ShoppingCartServices.ShopingCartViewModels;
using Services.ShoppingCartServices;
using System.Security.Claims;

namespace WebUI.Areas.Identity.Pages.Account.Manage
{
    public class PurchaseModel : PageModel
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IRateService rateService;
        private readonly ICartService cartService;

        public Dictionary<int, int> BookRatings { get; set; } = new Dictionary<int, int>();

        public PurchaseModel(IUnitOfWork unitOfWork, IRateService rateService, ICartService cartService)
        {
            _unitOfWork = unitOfWork;
            this.rateService = rateService;
            this.cartService = cartService;
        }

        public IEnumerable<PurchaseVM> purchaseTransactions = [];

        public async Task OnGetAsync() //  €ÌÌ— ≈·Ï OnGetAsync
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity!;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);

            //  √ﬂœ „‰ √‰ claim ·Ì” null
            if (claim != null)
            {
                var purchases =  _unitOfWork.GetRepository<PurchaseTransaction>()
                    .GetAllAsync(u => u.UserId == claim.Value && u.Status == "Complete", b => b.Book)
                    .GetAwaiter()
                    .GetResult();

                var result = new List<PurchaseVM>();

                foreach (var purchaseTransaction in purchases)
                {
                    var rate =  _unitOfWork.GetRepository<Rate>()
                        .GetAllAsync(x => x.BookId == purchaseTransaction.BookId)
                        .GetAwaiter()
                        .GetResult()
                        .FirstOrDefault();

                    if (rate != null)
                    {
                        result.Add(new PurchaseVM
                        {
                            PurchaseTransaction = purchaseTransaction,
                            Rate = rate.Rating
                        });
                    }
                    else
                    {
                        result.Add(new PurchaseVM
                        {
                            PurchaseTransaction = purchaseTransaction,
                            Rate = 0
                        });
                    }
                }

                purchaseTransactions = result;
            }
        }
        public async Task<IActionResult> OnPostPuracheAsync(int id)
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity!;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
            var book = ( await _unitOfWork.GetRepository<PurchaseTransaction>()
                .GetAllAsync(u => u.UserId == claim!.Value && u.BookId == id, b => b.Book, c => c.Book.Category))
                .FirstOrDefault();
            
              

            if (book != null)
            {
               
                if (book.Book.Stock == 0)
                {
                    TempData["ErrorMessage"] = "This book is not available for purchase.";
                    return RedirectToPage("Purchase");
                }

                try
                {
                    var shoppingCartItem = new CartVM
                    {
                        ApplicationUserId = claim.Value,
                        ProductId = book.BookId,
                        Count = 1,
                        IsBorrowed = false,
                    };
                    var existingCartItem = (await _unitOfWork.GetRepository<ShopingCart>()
                                       .GetAllAsync())
                                       .FirstOrDefault(x => x.ApplicationUserId == claim.Value && x.BookId == shoppingCartItem.ProductId && x.IsBorrowed == false);
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
                    return RedirectToPage("Purchase");
                }
            }
            else
            {
                return RedirectToPage("Purchase");

            }
        }

        public IActionResult OnPostSubmitRating()
        {
            var ratings = new Dictionary<int, int>();

            foreach (var key in Request.Form.Keys)
            {
                if (key.StartsWith("rating_"))
                {
                    var bookId = int.Parse(key.Replace("rating_", ""));
                    var rating = int.Parse(Request.Form[key]);

                    SaveRating(bookId, rating);
                }
            }

            return RedirectToPage("Purchase");
        }

        private async void SaveRating(int bookId, int rating)
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity!;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
            // ≈‰‘«¡ ﬂ«∆‰ Rate ÃœÌœ
            var newRate = new RateVM
            {
                BookId = bookId,
                Rating = rating,
                CreatedDate = DateTime.Now,
                UserId = claim.Value,
                ModifiedDate = DateTime.Now
            };
            var rate = _unitOfWork.GetRepository<Rate>()
                .GetAllAsync()
                .GetAwaiter()
                .GetResult()
                .FirstOrDefault(u=>u.UserId == newRate.UserId && u.BookId == newRate.BookId);

            if (rate != null)
            {
                rate.Rating = rating;
                await _unitOfWork.GetRepository<Rate>().UpdateAsync(rate);
                 _unitOfWork.SaveChangesAsync().GetAwaiter().GetResult();
            }
            else
            {
                await rateService.AddRateAsync(newRate);

            }
            


        }
        public class PurchaseVM
        {
            public PurchaseTransaction PurchaseTransaction { get; set; }
            public double Rate {  get; set; }
        }
    }
}
