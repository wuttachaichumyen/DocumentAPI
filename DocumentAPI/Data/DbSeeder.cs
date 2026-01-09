using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading.Tasks;

namespace DocumentAPI.Data
{
    public static class DbSeeder
    {
        public static async Task SeedRolesAndAdminAsync(IServiceProvider serviceProvider)
        {
            // 1. เรียกใช้ Service
            var userManager = serviceProvider.GetRequiredService<UserManager<IdentityUser>>();
            var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();

            // 2. สร้าง Roles (กำหนดตัวแปร Array ให้ชัดเจน)
            string[] roleNames = new string[] { "Admin", "Legal", "Insurance", "Auditor" };

            foreach (var roleName in roleNames)
            {
                var roleExist = await roleManager.RoleExistsAsync(roleName);
                if (!roleExist)
                {
                    await roleManager.CreateAsync(new IdentityRole(roleName));
                }
            }

            // 3. สร้าง User: Admin
            var adminUser = await userManager.FindByEmailAsync("admin@example.com");
            
            if (adminUser == null)
            {
                var newAdmin = new IdentityUser
                {
                    UserName = "admin",
                    Email = "admin@example.com",
                    EmailConfirmed = true
                };

                // สร้าง User พร้อม Password
                var createPowerUser = await userManager.CreateAsync(newAdmin, "Admin@1234");
                
                if (createPowerUser.Succeeded)
                {
                    // กำหนด Role Admin ให้ User นี้
                    await userManager.AddToRoleAsync(newAdmin, "Admin");
                }
            }
        }
    }
}