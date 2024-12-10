using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.Domains
{
    public class User:IdentityUser
    {
        public string FullName { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        [MaxLength(7)]
        public string Gender { get; set; }
        public string? Address { get; set; }
        public DateOnly? BirthDate { get; set; }
        public byte[]? ImageUrl { get; set; }
        public bool IsDeleted { get; set; }
        public ICollection<Rate>? Rates { get; set; }
        public ICollection<BorrowinTransaction>? BorrowinTransactions { get; set; }
        public ICollection<PurchaseTransaction>? PurchaseTransactions { get; set; }
    }
}
