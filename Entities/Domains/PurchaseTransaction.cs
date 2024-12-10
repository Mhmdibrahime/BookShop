using System.ComponentModel.DataAnnotations;

namespace Entities.Domains
{
    public class PurchaseTransaction
    {
        [Key]
        public int Id { get; set; }
        public string UserId { get; set; } = null!;
        public User User { get; set; } = null!;
        public int BookId { get; set; }
        public Book Book { get; set; } = null!;
        public DateTime PurchaseDate { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }
        public string? Status { get; set; }
        public decimal TotalAmount { get; set; }
    }
}
