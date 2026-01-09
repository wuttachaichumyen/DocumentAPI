using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace DocumentAPI.Models
{
    public class Document
    {
        public int Id { get; set; }
        
        [Required]
        public string Title { get; set; } = string.Empty;
        
        public string Category { get; set; } = string.Empty; // เช่น Contract, Invoice
        
        public string FilePath { get; set; } = string.Empty; // Path ที่เก็บไฟล์จริงบน Server/Cloud
        
        public string UploadedBy { get; set; } = string.Empty; // เก็บ UserID หรือ Username
        
        public bool IsActive { get; set; } = true; // Soft Delete: ถ้าลบจะเป็น false
        
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; } 
    }
}