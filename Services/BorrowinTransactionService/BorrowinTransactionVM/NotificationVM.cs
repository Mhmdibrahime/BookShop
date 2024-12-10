using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.BorrowinTransactionService.BorrowinTransactionVM
{
    public class NotificationVM
    {
        public string Message { get; set; }
        public DateTime CreatedAt { get; set; }
        public string UserId { get; set; }
        public DateTime DueDate { get; set; }
    }
}
