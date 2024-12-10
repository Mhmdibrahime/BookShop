using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace Entities.Domains
{
    public class Book : BaseEntity<int>
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
        [ValidateNever]
        public Author Author { get; set; }
        public int? PublisherId { get; set; }
        [ValidateNever]
        public Publisher Publisher { get; set; }
        public int? CategoryId { get; set; }
        [ValidateNever]
        public Category Category { get; set; }
        public ICollection<BorrowinTransaction> BorrowinTransactions { get; set; } = null!;
        public ICollection<PurchaseTransaction> PurchaseTransactions { get; set; } = null!;
    }
}
