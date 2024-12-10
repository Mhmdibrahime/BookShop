using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.BookServices.BookViewModels
{
    public class BookVM
    {
        public int Id { get; set; }
        public string? Title { get; set; }
        public string ISBN { get; set; }
        public string AuthorName { get; set; }
        public string PublisherName { get; set; }
        public string CategoryName { get; set; }
        public int Stock { get; set; }
        public decimal SalePrice {  get; set; }
        public decimal RentePrice {  get; set; }
        public string? ImagePath { get; set; }
        public bool IsDeleted { get; set; }
    }
}
