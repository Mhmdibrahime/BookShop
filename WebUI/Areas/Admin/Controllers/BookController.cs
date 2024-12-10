
using Entities.Domains;
using Entities.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Services.AuthorServices;
using Services.BookServices;
using Services.BookServices.BookViewModels;
using Services.CategoryServices;
using Services.PublisherServices;
using WebUI.Commponents;

namespace WebUI.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = RoleConst.AdminRole)]

    public class BookController(IBookService bookService, IAuthorService authorService, IPublisherService publisherService, ICategoryService categoryService, IUnitOfWork unitOfWork) : Controller
    {
        private readonly IBookService _bookService = bookService;
        private readonly IAuthorService _authorService = authorService;
        private readonly IPublisherService _publisherService = publisherService;
        private readonly ICategoryService _categoryService = categoryService;
        private readonly IUnitOfWork _unitOfWork = unitOfWork;

        public async Task<IActionResult> Index()=> View();

        [HttpGet]
        public async Task<IActionResult> GetAllBooks()
        {
            var books = await _bookService.GetAllAsync(
                b=>b.IsDeleted != true,
                b=>b.Author,
                c=>c.Category,
                p=>p.Publisher);
            return Json(new { data = books });
        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            var book = new BookCreateVM()
            {
                Categories = await _categoryService.GetAllAsync(),
                Authors = await _authorService.GetAllAsync(),
                Publishers = await _publisherService.GetAllAsync()
            };
            return View(book);
        }

        [HttpPost]
        [AutoValidateAntiforgeryToken]
        public async Task<IActionResult> Create(BookCreateVM AddBook ,IFormFile? coverFile)
        {
            
                if (coverFile != null && coverFile.Length > 0)
                {
                    var fileName = Path.GetFileNameWithoutExtension(coverFile.FileName);
                    var extension = Path.GetExtension(coverFile.FileName);
                    var newFileName = $"{fileName}_{DateTime.UtcNow.Ticks}{extension}";

                    var path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "covers", newFileName);

                    using (var stream = new FileStream(path, FileMode.Create))
                    {
                        await coverFile.CopyToAsync(stream);
                    }

                    AddBook.Cover = newFileName; 
                }

                await _bookService.AddAsync(AddBook);
               
                return RedirectToAction("Index");
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var book = await _bookService.GetByIdAsync(id);
            book.Categories = await _categoryService.GetAllAsync();
            book.Authors = await _authorService.GetAllAsync();
            book.Publishers = await _publisherService.GetAllAsync();
            return View(book);
        }

        [HttpPost]
        [AutoValidateAntiforgeryToken]
        public async Task<IActionResult> Edit(BookUpdateVM book, IFormFile? coverFile)
        {
            
                if (coverFile != null && coverFile.Length > 0)
                {
                    var fileName = Path.GetFileNameWithoutExtension(coverFile.FileName);
                    var extension = Path.GetExtension(coverFile.FileName);
                    var newFileName = $"{fileName}_{DateTime.UtcNow.Ticks}{extension}";

                    var path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "covers", newFileName);

                    
                    using (var stream = new FileStream(path, FileMode.Create))
                    {
                        await coverFile.CopyToAsync(stream);
                    }

                    if (!string.IsNullOrEmpty(book.Cover))
                    {
                        var oldCoverPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "covers", book.Cover);
                        if (System.IO.File.Exists(oldCoverPath))
                        {
                            System.IO.File.Delete(oldCoverPath);
                        }
                    }

                    book.Cover = newFileName;
                }

                await _bookService.UpdateAsync(book);
                return RedirectToAction("Index");
        }

        public async Task<IActionResult> DeleteBook(string id)
        {
            var book = await _unitOfWork.GetRepository<Book>().GetByIdAsync(Convert.ToInt32(id));

            if (book == null)
            {
                return Json(new { success = false, message = "Book not found" });
            }

            
            if (!string.IsNullOrEmpty(book.Cover)) 
            {
                var coverPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "covers", book.Cover);
                if (System.IO.File.Exists(coverPath))
                {
                    System.IO.File.Delete(coverPath);
                }
            }

            book.IsDeleted = true;

            await _unitOfWork.GetRepository<Book>().UpdateAsync(book);
            await _unitOfWork.SaveChangesAsync();

            return Json(new { success = true, message = "Book has been deleted" });
        }

        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            var book =  _unitOfWork.GetRepository<Book>().GetAllAsync(null,a=>a.Author,c=>c.Category,p=>p.Publisher)
                .GetAwaiter()
                .GetResult()
                .FirstOrDefault(x=>x.Id == id);
            if (book == null)
            {
                return NotFound();
            }

            
            return View(book);
        }

    }
}
