using Entities.Domains;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.PurchaseTransactionService.PurchaseTransactionVM
{
    public class PurchaseVM
    {
        public string UserId { get; set; } = null!;
        public int BookId { get; set; }
        public DateTime PurchaseDate { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }
        public decimal TotalAmount { get; set; }
        public string? Status { get; set; }
    }
}
