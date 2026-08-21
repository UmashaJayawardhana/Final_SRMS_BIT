using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace MvcApplication1.Models
{
    public class DashboardNotificationModel
    {
        public int NotificationId { get; set; }
        
        [Required(ErrorMessage = "Subject is required")]
        [StringLength(255)]
        public string Subject { get; set; }
        
        [Required(ErrorMessage = "Message Text is required")]
        [StringLength(1000)]
        public string MessageText { get; set; }
        
        public bool IsActive { get; set; }
        
        public DateTime CreatedDate { get; set; }
    }
}
