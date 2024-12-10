using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Entities.Domains;
using Services.BookServices.BookViewModels;
using Services.BorrowinTransactionService.BorrowinTransactionVM;
using Services.CategoryServices;
namespace Services.BorrowinTransactionService
{
    public interface IBorrowinTransactionService
    {
        Task<IEnumerable<BorrowinVM>> GetAllAsync(
            Expression<Func<BorrowinTransaction, bool>>? filter = null,
            params Expression<Func<BorrowinTransaction, object>>[] includes);
        Task<BorrowinVM> GetByIdAsync(object id);
        Task AddAsync(BorrowinVM entity);
      
    }
}
