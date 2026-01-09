using DocumentAPI.Data;
using DocumentAPI.Models;
using DocumentAPI.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using DocumentAPI.DTOs;

namespace DocumentAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class DocumentController : ControllerBase
    {
       private readonly AppDbContext _context;
        private readonly IWebHostEnvironment _environment;
        private readonly AuditService _auditService;

        public DocumentController(AppDbContext context, IWebHostEnvironment environment, AuditService auditService)
        {
            _context = context;
            _environment = environment;
            _auditService = auditService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll(
            [FromQuery] string? search, 
            [FromQuery] string? category,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10)
        {
            var query = _context.Documents.Where(d => d.IsActive); 

            if (!string.IsNullOrEmpty(search))
            {
                query = query.Where(d => d.Title.Contains(search) || d.UploadedBy.Contains(search));
            }

            if (!string.IsNullOrEmpty(category))
            {
                query = query.Where(d => d.Category == category);
            }

            var totalRecords = await query.CountAsync();

            var documents = await query
                .OrderByDescending(d => d.CreatedAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return Ok(new 
            { 
                TotalRecords = totalRecords, 
                Page = page, 
                PageSize = pageSize, 
                Data = documents 
            });
        }

        // 2. Get By ID
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var document = await _context.Documents.FindAsync(id);

            if (document == null || !document.IsActive)
                return NotFound();

            return Ok(document);
        }

        [HttpPost]
        public async Task<IActionResult> Upload([FromForm] CreateDocumentDto model)
        {
            // ... (Logic การเซฟไฟล์เหมือนเดิม) ...
            string filePath = "";
            if (model.File != null)
            {
                var uploadsFolder = Path.Combine(_environment.ContentRootPath, "uploads");
                if (!Directory.Exists(uploadsFolder)) Directory.CreateDirectory(uploadsFolder);
                var uniqueFileName = Guid.NewGuid().ToString() + "_" + model.File.FileName;
                filePath = Path.Combine(uploadsFolder, uniqueFileName);
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await model.File.CopyToAsync(stream);
                }
            }
            // ...

            var currentUserName = User.Identity?.Name ?? "Unknown";
            var newDoc = new Document
            {
                Title = model.Title,
                Category = model.Category,
                FilePath = filePath,
                UploadedBy = currentUserName,
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };

            _context.Documents.Add(newDoc);
            await _context.SaveChangesAsync();

            // ✅ บันทึก Audit Log: Create
            await _auditService.LogAsync("Create", "Documents", null, newDoc);

            return CreatedAtAction(nameof(GetById), new { id = newDoc.Id }, newDoc);
        }

        // 4. Soft Delete (เพิ่ม Audit Log)
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int id)
        {
            var document = await _context.Documents.FindAsync(id);
            if (document == null || !document.IsActive) return NotFound();

            // เก็บค่าเก่าไว้ก่อนลบ (เพื่อทำ Log)
            var oldData = new { document.Id, document.Title, document.IsActive };

            // Soft Delete
            document.IsActive = false;
            document.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            await _auditService.LogAsync("SoftDelete", "Documents", oldData, new { document.IsActive });

            return NoContent();
        }
    }
}