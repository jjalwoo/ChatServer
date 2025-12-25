using Microsoft.AspNetCore.Mvc;
using ChatServer.Data;
using ChatServer.Models;
using System.Security.Cryptography;
using System.Text;

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
        public IActionResult Signup(string email, string password, string nickname)
        {
            Console.Write("회원가입 요청이 들어왔습니다.");

            if(_db.Users.Any(u => u.Email == email))
            {
                Console.Write("이미 사용중인 이메일입니다.");
                return BadRequest("이미 사용중인 이메일입니다.");
            }

            var user = new User
            {
                Email = email,
                PasswordHash = HassPassword(password),
                Nickname = nickname,
                CreatedAt = DateTime.Now
            };

            _db.Users.Add(user);
            _db.SaveChanges();

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
