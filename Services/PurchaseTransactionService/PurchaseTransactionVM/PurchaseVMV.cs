using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.PurchaseTransactionService.PurchaseTransactionVM
{
    public class PurchaseVMV
    {
        public int Id { get; set; }
        public string UserName { get; set; } = null!;
        public string Phone {  get; set; } = null!;
        public string Address { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string BookTitle { get; set; } = null!;
        public decimal BookPrice {  get; set; }
        public int PurchaseCount {  get; set; }
        public decimal TotalPrice {  get; set; }
        public string Status {  get; set; } = null!;
        public DateTime PurchaseDate { get; set; }

    }
}
