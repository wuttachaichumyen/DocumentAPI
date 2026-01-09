using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace DocumentAPI.Models
{
    public class AuditLog
    {
        public int Id { get; set; }
        public string UserId { get; set; } = string.Empty; 
        public string Type { get; set; } = string.Empty;   // Create, Update, SoftDelete
        public string TableName { get; set; } = string.Empty;
        public string? OldValues { get; set; } // Keep Old JSON 
        public string? NewValues { get; set; } // Keep New JSON
        public DateTime Timestamp { get; set; } = DateTime.UtcNow; 
    }
}