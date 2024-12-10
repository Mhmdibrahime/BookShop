using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Entities.Domains;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.AspNetCore.Mvc.Rendering;
using Services.AuthorServices;
using Services.CategoryServices;
using Services.PublisherServices;

namespace Services.BookServices
{
    public class BookViewModel
    {
        public Book BookData { get; set; }
        public IEnumerable<SelectListItem>? Authors { get; set; }
        public IEnumerable<SelectListItem>? Categories { get; set; }
        public IEnumerable<SelectListItem>? Publishers { get; set; }
    }
}
