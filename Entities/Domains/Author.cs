using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.Domains
{
    public class Author : BaseEntity<int>
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public DateOnly BirthDate { get; set; }
        public string Email { get; set; }
        public ICollection<Book> Books { get; set; } = new List<Book>();
    }
}
