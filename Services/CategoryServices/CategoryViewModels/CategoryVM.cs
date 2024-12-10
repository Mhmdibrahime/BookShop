using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Services.PublisherServices;

namespace Services.CategoryServices.CategoryViewModels
{
    public class CategoryVM : CategoryUpdateVM
    {
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}
