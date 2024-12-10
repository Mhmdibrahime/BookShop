using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.ShoppingCartServices.ShopingCartViewModels
{
    public class CartView
    {
        public IEnumerable<ShopingCart> shopingCarts { get; set; }
        public decimal TotalPrice { get; set; }

    }
}
