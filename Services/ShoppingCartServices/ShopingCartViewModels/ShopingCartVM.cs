using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.ShoppingCartServices.ShopingCartViewModels
{
    public class ShopingCartVM
    {
        public ShopingCart shopingCart { get; set; }
        public int CountShoping
        {
            get; set;
        }
        public double RatingValue { get; set; }
    }
}