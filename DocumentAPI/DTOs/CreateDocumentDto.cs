using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace DocumentAPI.DTOs
{
    public class CreateDocumentDto
    {
        [Required]
        public string Title { get; set; } = string.Empty;

        [Required]
        public string Category { get; set; } = string.Empty;

        // รับไฟล์จริงๆ จากการอัปโหลด
        public IFormFile? File { get; set; }
    }
    public class UpdateDocumentDto
    {
        public string Title { get; set; }
        public string Category { get; set; }
    }
}