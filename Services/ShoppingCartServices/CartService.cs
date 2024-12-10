using Entities.Domains;
using Entities.Repositories;
using Services.ShoppingCartServices.ShopingCartViewModels;
using System.Linq.Expressions;
namespace Services.ShoppingCartServices
{
    public class CartService : ICartService
    {
        private readonly IUnitOfWork _unitOfWork;

        public CartService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

       
        public async Task AddCartAsync(CartVM entity)
        {
            var cart = new ShopingCart
            {
                ApplicationUserId = entity.ApplicationUserId,
                BookId = entity.ProductId,
                Count = entity.Count,
                IsBorrowed =entity.IsBorrowed
            };
            await _unitOfWork.GetRepository<ShopingCart>().AddAsync(cart);
            await _unitOfWork.SaveChangesAsync();
        }

       
    }
}
