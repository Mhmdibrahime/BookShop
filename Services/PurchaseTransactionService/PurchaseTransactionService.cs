using Entities.Domains;
using Entities.Repositories;
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
    public class PurchaseTransactionService(IUnitOfWork unitOfWork) : IPurchaseTransactionService
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;

        public async Task AddAsync(PurchaseVM entity)
        {
            var Borrowin = new PurchaseTransaction
            {
                UserId = entity.UserId,
                BookId = entity.BookId,
                Price = entity.Price,
                PurchaseDate = DateTime.Now,
                Quantity = entity.Quantity,
                TotalAmount = entity.TotalAmount,
                Status = entity.Status
            };
            await _unitOfWork.GetRepository<PurchaseTransaction>().AddAsync(Borrowin);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task<IEnumerable<PurchaseVM>> GetAllAsync(Expression<Func<PurchaseTransaction, bool>>? filter = null, params Expression<Func<PurchaseTransaction, object>>[] includes)
        {
            var books = await _unitOfWork.GetRepository<PurchaseTransaction>().GetAllAsync(filter, includes);
            return books.Select(b => new PurchaseVM()
            {
                UserId = b.UserId,
                BookId = b.BookId,
                Price = b.Price,
                PurchaseDate = DateTime.Now,
                Quantity = b.Quantity,
                TotalAmount = b.TotalAmount,
            }).ToList(); ;
        }

        public async Task<PurchaseVM> GetByIdAsync(object id)
        {
            var book = _unitOfWork.GetRepository<PurchaseTransaction>().GetAllAsync(x => x.Id == (int)id).Result.FirstOrDefault(x => x.Id == (int)id);

            if (book == null)
            {
                return null;
            }

            return new PurchaseVM
            {
                UserId = book.UserId,
                BookId = book.BookId,
                Price = book.Price,
                PurchaseDate = DateTime.Now,
                Quantity = book.Quantity,
                TotalAmount = book.TotalAmount,
            };
        }
    }
}
