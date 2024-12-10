using Entities.Domains;
using Entities.Repositories;
using Services.BorrowinTransactionService.BorrowinTransactionVM;
using System.Linq.Expressions;

namespace Services.BorrowinTransactionService
{
    public class BorrowinTransactionService(IUnitOfWork unitOfWork) : IBorrowinTransactionService

    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;

        public async Task<IEnumerable<BorrowinVM>> GetAllAsync(Expression<Func<BorrowinTransaction, bool>>? filter = null,  params Expression<Func<BorrowinTransaction, object>>[] includes)
        {
            var books = await _unitOfWork.GetRepository<BorrowinTransaction>().GetAllAsync(filter, includes);
            return books.Select(b => new BorrowinVM()
            {
              
            }).ToList(); ;
        }

      

        public async Task<BorrowinVM> GetByIdAsync(object id)
        {
            var book = _unitOfWork.GetRepository<BorrowinTransaction>().GetAllAsync(x => x.Id == (int)id).Result.FirstOrDefault(x=>x.Id==(int)id);

            if (book == null)
            {
                return null;
            }

            return new BorrowinVM
            {
               
            };

        }

        public async Task AddAsync(BorrowinVM entity)
        {
            var Borrowin = new BorrowinTransaction
            {
                UserId = entity.UserId,
                BookId = entity.BookId,
                BorrowDate = entity.BorrowDate,
                ReturnDate = entity.ReturnDate,
                Status = entity.Status,
                Price= entity.Price,
                TotalAmount= entity.TotalAmount,
                FineAmount = entity.FineAmount,
                Count = entity.Count,
            };
            await _unitOfWork.GetRepository<BorrowinTransaction>().AddAsync(Borrowin);
            await _unitOfWork.SaveChangesAsync();
        }  
        


        
    }
}
