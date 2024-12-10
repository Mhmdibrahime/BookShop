using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.ShoppingCartServices.ShopingCartViewModels
{
    public class CartVM
    {
        public string ApplicationUserId { get; set; }
        public int Count { get; set; }
        public int ProductId { get; set; }
        public bool IsBorrowed { get; set; }
    }
}
