using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Entities.Domains;
using Services.BookServices.BookViewModels;
using Services.CategoryServices;

namespace Services.BookServices
{
    public interface IBookService
    {
        Task<IEnumerable<BookVM>> GetAllAsync(
            Expression<Func<Book, bool>>? filter = null,
            params Expression<Func<Book, object>>[] includes);

        Task<BookUpdateVM> GetByIdAsync(object id);
        Task<Book> GetByIdBookAsync(object id);

        Task AddAsync(BookCreateVM entity);
        Task UpdateAsync(BookUpdateVM entity);
        Task DeleteAsync(BookViewModel entity);
        Task SoftDeleteAsync(BookViewModel entity);
        Task ExecuteUpdateAsync(BookViewModel entity);
        Task ExecuteDeleteAsync(BookViewModel entity);
    }
}
