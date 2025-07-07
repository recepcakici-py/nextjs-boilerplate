using System;
using System.ComponentModel.DataAnnotations;

namespace CalendarAppWPF.Models
{
    public class Event
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        
        [Required]
        public string Title { get; set; } = string.Empty;
        
        public string Description { get; set; } = string.Empty;
        
        [Required]
        public DateTime StartDateTime { get; set; }
        
        [Required]
        public DateTime EndDateTime { get; set; }
        
        public string Category { get; set; } = "Genel";
        
        public string Color { get; set; } = "#E8F4FD"; // Pastel blue default
        
        public bool IsAllDay { get; set; } = false;
        
        public bool HasReminder { get; set; } = true;
        
        public int ReminderMinutes { get; set; } = 15; // 15 minutes before
        
        public bool IsCompleted { get; set; } = false;
        
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        
        public DateTime? LastModified { get; set; }
    }
}
