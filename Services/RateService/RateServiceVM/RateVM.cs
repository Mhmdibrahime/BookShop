using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.RateService.RateServiceVM
{
    public class RateVM
    {
        public int BookId { get; set; }
        public string UserId { get; set; } = null!;
        public int Rating { get; set; }
        public string? Review { get; set; } = null!;
        public DateTime CreatedDate { get; set; } = DateTime.Now;
        public DateTime ModifiedDate { get; set; } = DateTime.Now;
    }
}
