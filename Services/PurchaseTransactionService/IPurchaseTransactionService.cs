using Entities.Domains;
using Services.BorrowinTransactionService.BorrowinTransactionVM;
using Services.PurchaseTransactionService.PurchaseTransactionVM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Services.PurchaseTransactionService
{
    public interface IPurchaseTransactionService
    {
        Task<IEnumerable<PurchaseVM>> GetAllAsync(
           Expression<Func<PurchaseTransaction, bool>>? filter = null,
           params Expression<Func<PurchaseTransaction, object>>[] includes);
        Task<PurchaseVM> GetByIdAsync(object id);
        Task AddAsync(PurchaseVM entity);

    }
}
