using Entities.Domains;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.ShoppingCartServices.ShopingCartViewModels
{
    public class ShoppingCartSummryVM
    {
        public ShopingCart shopingCart {  get; set; }
        public User applicationUser { get; set; }
    }
}
