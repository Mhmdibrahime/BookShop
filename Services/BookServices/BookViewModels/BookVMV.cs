using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.BookServices.BookViewModels
{
   public class BookVMV
    {
        public IEnumerable<BookVM> BookVMs { get; set; }
        public string CatigoryName {  get; set; }
    }
}
