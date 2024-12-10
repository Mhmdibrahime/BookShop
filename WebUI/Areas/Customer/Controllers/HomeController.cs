using DataAccess.Implementation;
using Entities.Domains;
using Entities.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Services.AuthorServices;
using Services.BookServices;
using Services.BorrowinTransactionService;
using Services.BorrowinTransactionService.BorrowinTransactionVM;
using Services.CategoryServices;
using Services.PublisherServices;
using Services.PurchaseTransactionService;
using Services.PurchaseTransactionService.PurchaseTransactionVM;
using Services.ShoppingCartServices;
using Services.ShoppingCartServices.ShopingCartViewModels;
using Stripe.Checkout;
using System.Security.Claims;
using System.Web.Helpers;
using X.PagedList;
using X.PagedList.Extensions;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace WebUI.Areas.Customer.Controllers
{
    [Area("Customer")]
    public class HomeController(
        ICartService cartService,
        IUnitOfWork unitOfWork,
        IBorrowinTransactionService borrowinTransactionService,
        IPurchaseTransactionService purchaseTransactionService,
        IBookService bookService) : Controller
    {
        private readonly IUnitOfWork unitOfWork = unitOfWork;
        private readonly ICartService cartService = cartService;
        private readonly IBorrowinTransactionService borrowinTransactionService = borrowinTransactionService;
        private readonly IPurchaseTransactionService purchaseTransactionService = purchaseTransactionService;
        private readonly IBookService bookService = bookService;


        //------------------------Start Get Functions----------------------------

        public async Task<IActionResult> Index(int? page, string? filter)
        {
            var pageNumber = page ?? 1;
            var pageSize = 8;

            if (filter != null)
            {
                var books = await bookService.GetAllAsync( f => f.Category.Name == filter && f.IsDeleted == false, c => c.Category, a => a.Author);
                return View(books.ToPagedList(pageNumber, pageSize));
            }
            else
            {
                var books = await bookService.GetAllAsync(x => x.IsDeleted == false, c => c.Category, a => a.Author);
                return View(books.ToPagedList(pageNumber, pageSize));
            }

        }
        [Authorize]
        public async Task<IActionResult> Cart()
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity!;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
            if (unitOfWork == null)
                throw new Exception("unitOfWork is not initialized.");

            var shoppingCartRepository = unitOfWork.GetRepository<ShopingCart>();


            var shoppingCartItems = await shoppingCartRepository.GetAllAsync(
                x => x.ApplicationUserId == claim.Value,
                x => x.Book
            );
            decimal totalPrice = 0;
            foreach (var item in shoppingCartItems)
            {
                if (item.IsBorrowed)
                    totalPrice += item.Count * item.Book.RentPrice;

                else
                    totalPrice += item.Count * item.Book.SalePrice;
            }
            var cartView = new CartView
            {
                shopingCarts = shoppingCartItems,
                TotalPrice = totalPrice
            };
            return View(cartView);
        }
        public async Task<IActionResult> GetNotifications()
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity!;
            var claim = claimsIdentity?.FindFirst(ClaimTypes.NameIdentifier);

            if (claim == null)
            {
                return Json(new { success = false, message = "User not authenticated." });
            }

            var notifications = await unitOfWork.GetRepository<Notification>()
                .GetAllAsync(u => u.UserId == claim.Value && u.IsRead ==false);

            return Json(new { success = true, data = notifications });
        }
        public async Task<IActionResult> MarkNotificationAsRead(int? notificationId)
        {
            if (notificationId != null)
            {
                try
                {
                    var notification = await unitOfWork.GetRepository<Notification>()
                     .GetByIdAsync(notificationId!);

                    if (notification != null)
                    {
                        notification.IsRead = true;

                        await unitOfWork.GetRepository<Notification>()
                           .UpdateAsync(notification!);
                        await unitOfWork.SaveChangesAsync();
                        return Json(new { success = true, message = "successfully" });

                    }
                }
                catch(Exception ex)
                {
                    return Json(new { success = false, message = $"{ex.Message}" });
                }
            }

            return Json(new { success = false, message = "there is error." });

        }
        public async Task<IActionResult> About()
        {
            return View();
        }


        [HttpGet]
        public async Task<IActionResult> Detials(int id)
        {
            var book = unitOfWork.GetRepository<Book>().GetAllAsync(x => x.Id == id, a => a.Author, c => c.Category, p => p.Publisher)
                .GetAwaiter()
                .GetResult()
                .FirstOrDefault();

            var rateThisBook = unitOfWork.GetRepository<Rate>().GetAllAsync(b => b.BookId == id)
                .GetAwaiter()
                .GetResult();

            ShopingCartVM obj;
            if (rateThisBook.Count() != 0)
            {
                var rate = rateThisBook.Average(c => c.Rating);
                obj = new()
                {
                    shopingCart = new ShopingCart()
                    {
                        BookId = id,
                        Book = book!,
                        Count = 1,
                    },
                    RatingValue = rate

                };
                return View(obj);

            }

            obj = new()
            {
                shopingCart = new ShopingCart()
                {
                    BookId = id,
                    Book = book,
                    Count = 1,
                },
                RatingValue = 0

            };

            return View(obj);
        }

        //------------------------End Get Functions----------------------------



        //------------------------Strart Post Functions----------------------------




        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> Detials(ShopingCart shopingCart)
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity!;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);

            var book = await unitOfWork.GetRepository<Book>().GetByIdAsync(shopingCart.BookId);
            if (book == null)
            {
                TempData["ErrorMessage"] = "Book not found.";
                return RedirectToAction("Index");
            }

            int availableStock = book.Stock;

            if (availableStock < shopingCart.Count)
            {
                TempData["ErrorMessage"] = "The requested quantity exceeds the available stock.";
                return RedirectToAction("Index");
            }

            shopingCart.ApplicationUser = await unitOfWork.GetRepository<User>().GetByIdAsync(claim.Value);
            shopingCart.Book = book;

            var shoping = new CartVM
            {
                ApplicationUserId = claim.Value,
                ProductId = shopingCart.BookId,
                Count = shopingCart.Count,
                IsBorrowed = shopingCart.IsBorrowed,
            };

            var existingCartItem = (await unitOfWork.GetRepository<ShopingCart>()
                                       .GetAllAsync())
                                       .FirstOrDefault(x => x.ApplicationUserId == claim.Value && x.BookId == shoping.ProductId && x.IsBorrowed == shoping.IsBorrowed);

            if (existingCartItem == null)
            {
                if (shopingCart.Count > availableStock)
                {
                    TempData["ErrorMessage"] = "The quantity you requested exceeds the available stock.";
                    return Redirect("/Customer/Home/Index");
                }
                await cartService.AddCartAsync(shoping);
            }
            else
            {
                int newTotalCount = existingCartItem.Count + shopingCart.Count;
                if (newTotalCount > availableStock)
                {
                    TempData["ErrorMessage"] = "The total quantity you requested exceeds the available stock.";
                    return Redirect("/Customer/Home/Index");
                }
                IncressCountProduct(existingCartItem, shopingCart.Count, shopingCart.IsBorrowed);
                await unitOfWork.SaveChangesAsync();
            }

            return Redirect("/Customer/Home/Index");
        }

        [HttpPost]
        public async Task<JsonResult> AddPlus(int id)
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity!;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
            var shoppingCartItem = await unitOfWork.GetRepository<ShopingCart>().GetByIdAsync(id);
            if (shoppingCartItem != null)
            {
                var book = await unitOfWork.GetRepository<Book>().GetByIdAsync(shoppingCartItem.BookId);
                if (book != null && shoppingCartItem.Count < book.Stock)
                {
                    shoppingCartItem.Count += 1;
                    await unitOfWork.GetRepository<ShopingCart>().UpdateAsync(shoppingCartItem);
                    await unitOfWork.SaveChangesAsync();

                    var cartItems = await unitOfWork.GetRepository<ShopingCart>().GetAllAsync(u => u.ApplicationUserId == claim.Value, b => b.Book);
                    var updatedTotalPrice = cartItems.Sum(item =>
                        item.IsBorrowed ? item.Book.RentPrice * item.Count : item.Book.SalePrice * item.Count);

                    return Json(new { count = shoppingCartItem.Count, success = true, totalPrice = updatedTotalPrice });
                }
                else
                {
                    return Json(new { success = false, message = "Not enough copies available" });
                }
            }
            return Json(new { success = false, message = "Item not found" });
        }
        [HttpPost]
        public async Task<JsonResult> AddMins(int id)
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity!;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
            var shoping = await unitOfWork.GetRepository<ShopingCart>().GetByIdAsync(id);
            if (shoping != null && shoping.Count > 1)
            {
                var book = await unitOfWork.GetRepository<Book>().GetByIdAsync(shoping.BookId);
                shoping.Count -= 1;
                await unitOfWork.GetRepository<ShopingCart>().UpdateAsync(shoping);
                await unitOfWork.SaveChangesAsync();

                var cartItems = await unitOfWork.GetRepository<ShopingCart>().GetAllAsync(u => u.ApplicationUserId == claim.Value, b => b.Book);
                var updatedTotalPrice = cartItems.Sum(item =>
                    item.IsBorrowed ? item.Book.RentPrice * item.Count : item.Book.SalePrice * item.Count);

                return Json(new { count = shoping.Count, success = true, totalPrice = updatedTotalPrice });
            }
            else if (shoping != null && shoping.Count <= 1)
            {
                await unitOfWork.GetRepository<ShopingCart>().DeleteAsync(shoping);
                await unitOfWork.SaveChangesAsync();
                return Json(new { count = 0, success = true });
            }
            return Json(new { success = false, message = "Item not found" });
        }

        [HttpPost]
        public async Task<JsonResult> DeleteItem(int id)
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity!;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
            var shoping = await unitOfWork.GetRepository<ShopingCart>().GetByIdAsync(id);
            if (shoping != null)
            {
                await unitOfWork.GetRepository<ShopingCart>().DeleteAsync(shoping);
                await unitOfWork.SaveChangesAsync();

                var cartItems = await unitOfWork.GetRepository<ShopingCart>().GetAllAsync(u => u.ApplicationUserId == claim.Value, b => b.Book);
                var updatedTotalPrice = cartItems.Sum(item =>
                    item.IsBorrowed ? item.Book.RentPrice * item.Count : item.Book.SalePrice * item.Count);

                return Json(new { success = true, totalPrice = updatedTotalPrice });
            }
            return Json(new { success = false, message = "Item not found" });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CheckOut()
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity!;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);

            var shopingCarts = await unitOfWork.GetRepository<ShopingCart>().GetAllAsync(p => p.ApplicationUserId == claim!.Value, b => b.Book, u => u.ApplicationUser);

            var transactiontsBorrowin = new List<BorrowinVM>();
            var transactiontsPurchase = new List<PurchaseVM>();
            foreach (var item in shopingCarts)
            {
                if (item.IsBorrowed)
                {
                    transactiontsBorrowin.Add(new BorrowinVM
                    {
                        UserId = item.ApplicationUserId,
                        BookId = item.BookId,
                        BorrowDate = DateTime.Now,
                        DueDate = DateTime.Now.AddDays(15),
                        FineAmount = 0,
                        TotalAmount = item.Count * item.Book.RentPrice,
                        ReturnDate = DateTime.Now.AddDays(15),
                        Price = item.Book.RentPrice,
                        Status = "Pending",
                        Count = item.Count,
                    });

                }
                else
                {
                    transactiontsPurchase.Add(new PurchaseVM
                    {
                        UserId = item.ApplicationUserId,
                        BookId = item.BookId,
                        Price = item.Book.SalePrice,
                        TotalAmount = item.Book.SalePrice * item.Count,
                        PurchaseDate = DateTime.Now,
                        Quantity = item.Count,
                        Status = "Pending"

                    });
                }
            }


            foreach (var item in transactiontsBorrowin)
            {
                await borrowinTransactionService.AddAsync(item);
            }
            foreach (var item in transactiontsPurchase)
            {
                await purchaseTransactionService.AddAsync(item);
            }


            var domain = "http://librarytapp.runasp.net";

            var options = new SessionCreateOptions
            {
                LineItems = new List<SessionLineItemOptions>(),
                Mode = "payment",
                SuccessUrl = domain + $"/Customer/Home/OrderConfirmation",
                CancelUrl = domain + $"/Customer/Home/Cart",
            };

            foreach (var item in shopingCarts)
            {

                var sessionlineoptions = new SessionLineItemOptions
                {
                    PriceData = new SessionLineItemPriceDataOptions
                    {

                        UnitAmount = item.IsBorrowed == true ? (long)(item.Book.RentPrice) * 100 : (long)(item.Book.SalePrice) * 100,
                        Currency = "usd",
                        ProductData = new SessionLineItemPriceDataProductDataOptions
                        {
                            Name = Title(item.Book.Title, item.IsBorrowed)
                        },
                    },
                    Quantity = item.Count,
                };
                options.LineItems.Add(sessionlineoptions);



            }

            try
            {
                var service = new SessionService();
                Session session = service.Create(options);
                Response.Headers.Add("Location", session.Url);
                return new StatusCodeResult(303);
            }
            catch
            {
                return NotFound();
            }

        }

        [Authorize]
        public async Task<IActionResult> OrderConfirmation()
        {
            try
            {
                var claimsIdentity = (ClaimsIdentity)User.Identity!;
                var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);

                List<ShopingCart> shopingCarts = unitOfWork.GetRepository<ShopingCart>()
                    .GetAllAsync(x => x.ApplicationUserId == claim.Value)
                    .GetAwaiter()
                    .GetResult()
                    .ToList();

                
                foreach (var item in shopingCarts)
                {
                    unitOfWork.GetRepository<ShopingCart>().DeleteAsync(item)
                        .GetAwaiter()
                        .GetResult();
                    var book = unitOfWork.GetRepository<Book>()
                        .GetByIdAsync(item.BookId)
                        .GetAwaiter()
                        .GetResult();
                    book.Stock -= item.Count;
                    unitOfWork.GetRepository<Book>()
                        .UpdateAsync(book)
                        .GetAwaiter()
                        .GetResult();
                }
                    
                unitOfWork.SaveChangesAsync().GetAwaiter().GetResult();

                return View();
            }
            catch (Exception ex)
            {
                return Redirect("/Customer/Home/PageError");
            }
        }
        public IActionResult PageError()
        {
            return View();
        }




        //------------------------End Post Functions----------------------------



        // Functions Helper.......................
        public string Title(string title, bool IsBorrowed)
        {
            var value = title;
            if (IsBorrowed)
            {
                value += $" ( Borrowed )";
            }
            else
            {
                value += $" ( Purchased )";
            }

            return value;
        }

        public async void IncressCountProduct(ShopingCart entity, int value, bool IsStatus)
        {
            var shop = unitOfWork.GetRepository<ShopingCart>().GetAllAsync().GetAwaiter().GetResult().FirstOrDefault(e => e.Id == entity.Id && e.IsBorrowed == IsStatus);
            var book = await unitOfWork.GetRepository<Book>().GetByIdAsync(shop.BookId);
            if (shop != null)
            {
                shop.Count += value;
                await unitOfWork.GetRepository<ShopingCart>().UpdateAsync(shop);
            }
        }

    }

}
