using Entities.Domains;
using Entities.Repositories;
using Services.AuthorServices;
using Services.CategoryServices;
using Services.PublisherServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Services.BookServices.BookViewModels;

namespace Services.BookServices
{
    public class BookService : IBookService
    {
        private readonly IUnitOfWork _unitOfWork;

        public BookService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<IEnumerable<BookVM>> GetAllAsync(Expression<Func<Book, bool>>? filter = null,
            params Expression<Func<Book, object>>[] includes)
        {

            var books = await _unitOfWork.GetRepository<Book>().GetAllAsync(filter,includes);
            return books.Select(b => new BookVM()
            {
                Id = b.Id,
                Title = b.Title,
                ISBN = b.ISBN,
                Stock = b.Stock,
                AuthorName = b.Author?.Name ?? "Unknown",
                CategoryName = b.Category?.Name ?? "Unknown",
                PublisherName = b.Publisher?.Name ?? "Unknown",
                ImagePath = b.Cover,
                SalePrice = b.SalePrice,
                RentePrice = b.RentPrice,
                IsDeleted = b.IsDeleted
            }).ToList(); ;
        }

        public async Task<Book> GetByIdBookAsync(object id)
        {
            var book = _unitOfWork.GetRepository<Book>().GetAllAsync(x => x.Id == (int)id,
                b => b.Author,
                b => b.Category,
                b => b.Publisher).Result.FirstOrDefault(x => x.Id == (int)id);

            if (book == null)
            {
                return null; // or handle the null case appropriately
            }

            return book;

        }
        public async Task<BookUpdateVM> GetByIdAsync(object id)
        {
            var book = _unitOfWork.GetRepository<Book>().GetAllAsync(x => x.Id == (int)id).Result.FirstOrDefault();

            if (book == null)
            {
                return null; // or handle the null case appropriately
            }

            return new BookUpdateVM
            {
                Id = book.Id,
                Title = book.Title,
                Description = book.Description,
                ISBN = book.ISBN,
                PublishedYear = book.PublishedYear,
                SalePrice = book.SalePrice,
                RentPrice = book.RentPrice,
                Stock = book.Stock,
                Cover = book.Cover,
                AuthorId = book.AuthorId,
                PublisherId = book.PublisherId,
                CategoryId = book.CategoryId,
                AuthorName = book.Author?.Name,
                PublisherName = book.Publisher?.Name,
                CategoryName = book.Category?.Name
            };

        }

        public async Task AddAsync(BookCreateVM entity)
        {
            var book = new Book
            {
                Title = entity.Title,
                Description = entity.Description,
                ISBN = entity.ISBN,
                PublishedYear = entity.PublishedYear,
                SalePrice = entity.SalePrice,
                RentPrice = entity.RentPrice,
                Stock = entity.Stock,
                Cover = entity.Cover,
                AuthorId = entity.AuthorId,
                PublisherId = entity.PublisherId,
                CategoryId = entity.CategoryId
            };
            await _unitOfWork.GetRepository<Book>().AddAsync(book);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task UpdateAsync(BookUpdateVM entity)
        {
            var book = new Book
            {
                Id = entity.Id,
                Title = entity.Title,
                Description = entity.Description,
                ISBN = entity.ISBN,
                PublishedYear = entity.PublishedYear,
                SalePrice = entity.SalePrice,
                RentPrice = entity.RentPrice,
                Stock = entity.Stock,
                Cover = entity.Cover,
                AuthorId = entity.AuthorId,
                PublisherId = entity.PublisherId,
                CategoryId = entity.CategoryId,
                ModifiedDate = DateTime.UtcNow
            };
            await _unitOfWork.GetRepository<Book>().UpdateAsync(book);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task DeleteAsync(BookViewModel entity)
        {
            var book = new Book
            {
                Id = entity.BookData.Id,
                Title = entity.BookData.Title,
                Description = entity.BookData.Description,
                ISBN = entity.BookData.ISBN,
                PublishedYear = entity.BookData.PublishedYear,
                SalePrice = entity.BookData.SalePrice,
                RentPrice = entity.BookData.RentPrice,
                Stock = entity.BookData.Stock,
                Cover = entity.BookData.Cover,
                AuthorId = entity.BookData.AuthorId,
                PublisherId = entity.BookData.PublisherId,
                CategoryId = entity.BookData.CategoryId
            };
            await _unitOfWork.GetRepository<Book>().DeleteAsync(book);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task SoftDeleteAsync(BookViewModel entity)
        {
            var book = new Book
            {
                Id = entity.BookData.Id,
                Title = entity.BookData.Title,
                Description = entity.BookData.Description,
                ISBN = entity.BookData.ISBN,
                PublishedYear = entity.BookData.PublishedYear,
                SalePrice = entity.BookData.SalePrice,
                RentPrice = entity.BookData.RentPrice,
                Stock = entity.BookData.Stock,
                Cover = entity.BookData.Cover,
                AuthorId = entity.BookData.AuthorId,
                PublisherId = entity.BookData.PublisherId,
                CategoryId = entity.BookData.CategoryId,
                IsDeleted = true,
                DeletedDate = DateTime.Now
            };
            await _unitOfWork.GetRepository<Book>().UpdateAsync(book);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task ExecuteUpdateAsync(BookViewModel entity)
        {
            await _unitOfWork.GetRepository<Book>().GetDbSet()
                .Where(b => b.Id == entity.BookData.Id)
                .ExecuteUpdateAsync(b => b
                    .SetProperty(x => x.Title, entity.BookData.Title)
                    .SetProperty(x => x.Description, entity.BookData.Description)
                    .SetProperty(x => x.ISBN, entity.BookData.ISBN)
                    .SetProperty(x => x.PublishedYear, entity.BookData.PublishedYear)
                    .SetProperty(x => x.SalePrice, entity.BookData.SalePrice)
                    .SetProperty(x => x.RentPrice, entity.BookData.RentPrice)
                    .SetProperty(x => x.Stock, entity.BookData.Stock)
                    .SetProperty(x => x.Cover, entity.BookData.Cover)
                    .SetProperty(x => x.ModifiedDate, DateTime.UtcNow)
                );
        }

        public async Task ExecuteDeleteAsync(BookViewModel entity)
        {
            await _unitOfWork.GetRepository<Book>().GetDbSet()
                .Where(b => b.Id == entity.BookData.Id)
                .ExecuteDeleteAsync();
        }
    }
}
