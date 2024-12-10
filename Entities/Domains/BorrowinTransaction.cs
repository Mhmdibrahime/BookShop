
using System.ComponentModel.DataAnnotations;

namespace Entities.Domains
{
    public class BorrowinTransaction
    {
        [Key]
        public int Id { get; set; }
        public string UserId { get; set; } = null!;
        public User User { get; set; } = null!;
        public int BookId {  get; set; }
        public Book Book { get; set; } = null!;
        public DateTime BorrowDate { get; set; }
        public DateTime ReturnDate { get; set; }
        [MaxLength(20)]
        public string Status {  get; set; } = null!;
        public decimal Price { get; set; }
        public decimal TotalAmount { get; set; }
        public decimal FineAmount {  get; set; }    
        public int Count { get; set; }
    }
}
