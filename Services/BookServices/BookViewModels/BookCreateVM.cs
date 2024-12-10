using Entities.Domains;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Services.AuthorServices.AuthorViewModels;
using Services.CategoryServices.CategoryViewModels;
using Services.PublisherServices.PublisherViewModels;

namespace Services.BookServices.BookViewModels
{
    public class BookCreateVM
    {
        public string? Title { get; set; }
        public string? Description { get; set; }
        public string? ISBN { get; set; }
        public int PublishedYear { get; set; }
        public decimal SalePrice { get; set; }
        public decimal RentPrice { get; set; }
        public int Stock { get; set; }
        [ValidateNever]
        public string? Cover { get; set; }
        public int? AuthorId { get; set; }
        public int? PublisherId { get; set; }
        public int? CategoryId { get; set; }
        [ValidateNever]
        public IEnumerable<CategoryUpdateVM> Categories { get; set; }
        [ValidateNever]
        public IEnumerable<AuthorUpdateVM> Authors { get; set; }
        [ValidateNever]
        public IEnumerable<PublisherUpdateVM> Publishers { get; set; }
    }
}
