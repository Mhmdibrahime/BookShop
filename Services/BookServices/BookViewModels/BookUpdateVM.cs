using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.BookServices.BookViewModels
{
    public class BookUpdateVM : BookCreateVM
    {
        public int Id { get; set; }
        public string AuthorName { get; set; }
        public string PublisherName { get; set; }
        public string CategoryName { get; set; }
    }
}
