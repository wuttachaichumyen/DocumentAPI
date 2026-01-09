using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DocumentAPI.Data;
using Microsoft.AspNetCore.Http;
using DocumentAPI.Models;
using System.Security.Claims;
using System.Text.Json;

namespace DocumentAPI.Services
{
    public class AuditService
    {
        private readonly AppDbContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public AuditService(AppDbContext context, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task LogAsync(string actionType, string tableName, object? oldValues, object? newValues)
        {
            // ดึง User ที่กำลังใช้งานอยู่
            var userId = _httpContextAccessor.HttpContext?.User?.Identity?.Name ?? "System";

            var audit = new AuditLog
            {
                UserId = userId,
                Type = actionType, // เช่น "Create", "Update", "SoftDelete"
                TableName = tableName,
                Timestamp = DateTime.UtcNow,
                OldValues = oldValues != null ? JsonSerializer.Serialize(oldValues) : null,
                NewValues = newValues != null ? JsonSerializer.Serialize(newValues) : null
            };

            _context.AuditLogs.Add(audit);
            await _context.SaveChangesAsync();
        }
    }
}