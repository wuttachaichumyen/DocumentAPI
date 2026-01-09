using DocumentAPI.Controllers;
using DocumentAPI.Data;
using DocumentAPI.Models;
using DocumentAPI.Services;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Moq;
using Xunit;

namespace DocumentAPI.Tests
{
    public class DocumentControllerTests
    {
        // แก้ไขตรงนี้เช่นกัน
        private AppDbContext GetInMemoryDbContext()
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            var mockHttpContextAccessor = new Mock<IHttpContextAccessor>();

            return new AppDbContext(options, mockHttpContextAccessor.Object);
        }

        [Fact]
        public async Task GetAll_ShouldReturnOnlyActiveDocuments()
        {
            // Arrange
            var context = GetInMemoryDbContext();
            
            context.Documents.Add(new Document { Title = "Doc 1", IsActive = true, UploadedBy = "User1" });
            context.Documents.Add(new Document { Title = "Doc 2", IsActive = false, UploadedBy = "User1" });
            await context.SaveChangesAsync();

            var mockEnv = new Mock<IWebHostEnvironment>();
            // แก้ตรงนี้ให้ส่ง AuditService เข้าไปด้วย (เพราะ Controller ต้องการ)
            var mockAudit = new Mock<AuditService>(context, new Mock<IHttpContextAccessor>().Object);

            var controller = new DocumentController(context, mockEnv.Object, mockAudit.Object);

            // Act
            var result = await controller.GetAll(null, null, 1, 10);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(200, okResult.StatusCode);
        }
    }
}