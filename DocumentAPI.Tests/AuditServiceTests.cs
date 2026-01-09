using DocumentAPI.Data;
using DocumentAPI.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Moq;
using System.Security.Claims;
using Xunit;

namespace DocumentAPI.Tests
{
    public class AuditServiceTests
    {
        // แก้ไขตรงนี้: สร้าง Mock IHttpContextAccessor ส่งเข้าไปด้วย
        private AppDbContext GetInMemoryDbContext()
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;
            
            // สร้าง Mock เปล่าๆ ส่งเข้าไปเพื่อให้ Constructor ไม่แดง
            var mockHttpContextAccessor = new Mock<IHttpContextAccessor>();
            
            return new AppDbContext(options, mockHttpContextAccessor.Object);
        }

        [Fact]
        public async Task LogAsync_ShouldAddAuditLogToDatabase()
        {
            // Arrange
            var context = GetInMemoryDbContext();
            
            var mockHttpContextAccessor = new Mock<IHttpContextAccessor>();
            var contextUser = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
            {
                new Claim(ClaimTypes.Name, "TestUser")
            }, "mock"));

            mockHttpContextAccessor.Setup(h => h.HttpContext).Returns(new DefaultHttpContext
            {
                User = contextUser
            });

            var service = new AuditService(context, mockHttpContextAccessor.Object);

            // Act
            await service.LogAsync("Create", "TestTable", null, new { Value = "New" });

            // Assert
            var log = await context.AuditLogs.FirstOrDefaultAsync();
            Assert.NotNull(log);
            Assert.Equal("TestUser", log.UserId);
            Assert.Equal("Create", log.Type);
        }
    }
}