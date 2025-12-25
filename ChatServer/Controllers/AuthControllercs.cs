using System.Security.Cryptography;
using System.Text;
using ChatServer.Data;
using ChatServer.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

// 회원가입 API
namespace ChatServer.Controllers
{
    [ApiController]
    [Route("auth")]

    public class AuthControllercs : ControllerBase
    {
        private readonly AppDbContext _db;

        public AuthControllercs(AppDbContext db)
        {
            _db = db;
        }

        [HttpPost("signup")]
        public async Task<IActionResult> Signup(string email, string password, string nickname)
        {
            Console.Write("회원가입 요청이 들어왔습니다.");

            bool exists = await _db.Users.AnyAsync(u => u.Email == email);

            if (exists)
            {
                Console.WriteLine("이미 사용 중인 이메일입니다.");
                return BadRequest("이미 사용 중인 이메일입니다.");
            }

            var user = new User
            {
                Email = email,
                PasswordHash = HassPassword(password),
                Nickname = nickname,
                CreatedAt = DateTime.Now
            };

            _db.Users.Add(user);

            await _db.SaveChangesAsync();

            Console.Write("회원가입이 완료되었습니다.");

            return Ok(new
            {
                userId = user.Id,
                emali = user.Email,
                nickname = user.Nickname,
                createdAt = user.CreatedAt
            });

        }

        // 비번 해시로 바꾸는 함수
        private string HassPassword(string password)
        {
            using (var sha = SHA256.Create())
            {
                var bytes = sha.ComputeHash(Encoding.UTF8.GetBytes(password));
                return Convert.ToBase64String(bytes);
            }
        }
    }
}
