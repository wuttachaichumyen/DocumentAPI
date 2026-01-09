using DocumentAPI.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using DocumentAPI.DTOs;

namespace DocumentAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IConfiguration _configuration;

        public AuthController(UserManager<IdentityUser> userManager, IConfiguration configuration)
        {
            _userManager = userManager;
            _configuration = configuration;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto model)
        {
            // 1. เช็คว่ามี User นี้ไหม
            var user = await _userManager.FindByNameAsync(model.Username);
            if (user == null) return Unauthorized("User not found");

            // 2. เช็ค Password
            if (!await _userManager.CheckPasswordAsync(user, model.Password))
                return Unauthorized("Password incorrect");

            // 3. ดึง Role ของ User ออกมา
            var userRoles = await _userManager.GetRolesAsync(user);

            // 4. สร้าง Claims (ข้อมูลที่จะฝังใน Token)
            var authClaims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.UserName!),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            };

            // ใส่ Role เข้าไปใน Token
            foreach (var role in userRoles)
            {
                authClaims.Add(new Claim(ClaimTypes.Role, role));
            }

            // 5. สร้าง Key สำหรับเซ็นชื่อ (ต้องตรงกับใน Program.cs)
            // หมายเหตุ: เพื่อความง่ายผม hardcode ให้ตรงกับที่คุณใช้ใน Program.cs ก่อน
            var authSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("SuperSecretKey12345678901234567890"));

            var token = new JwtSecurityToken(
                issuer: "DocumentAPI",
                audience: "DocumentAPI",
                expires: DateTime.Now.AddHours(3),
                claims: authClaims,
                signingCredentials: new SigningCredentials(authSigningKey, SecurityAlgorithms.HmacSha256)
            );

            return Ok(new
            {
                token = new JwtSecurityTokenHandler().WriteToken(token),
                expiration = token.ValidTo
            });
        }
    }
}