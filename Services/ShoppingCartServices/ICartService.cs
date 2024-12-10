

using Entities.Domains;
using Services.ShoppingCartServices.ShopingCartViewModels;
using System.Linq.Expressions;

namespace Services.ShoppingCartServices
{
    public interface ICartService
    {
    
        Task AddCartAsync(CartVM entity);
    }
}
