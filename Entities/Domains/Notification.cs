using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.Domains
{
    public class Notification
    {
            public int Id { get; set; } // معرف الإشعار
            public string Message { get; set; } // نص الإشعار
            public DateTime CreatedAt { get; set; } // تاريخ إنشاء الإشعار
            public bool IsRead { get; set; } // حالة الإشعار (مقروء أو غير مقروء)
            public string UserId { get; set; } // معرف المستخدم الذي يتلقى الإشعار
            public string BookName {  get; set; }
            public DateTime DueDate { get; set; } // تاريخ الاستحقاق

        
    }
}
