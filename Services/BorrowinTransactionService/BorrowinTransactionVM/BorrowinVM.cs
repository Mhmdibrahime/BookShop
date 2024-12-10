using Entities.Domains;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.BorrowinTransactionService.BorrowinTransactionVM
{
    public class BorrowinVM
    {
        public string UserId { get; set; } = null!;
        public int BookId { get; set; }
        public DateTime BorrowDate { get; set; }
        public DateTime DueDate { get; set; }
        public DateTime ReturnDate { get; set; }
        [MaxLength(20)]
        public string Status { get; set; } = null!;
        public decimal Price { get; set; }
        public decimal TotalAmount { get; set; }
        public decimal FineAmount { get; set; }
        public int Count { get; set; }
    }
}
