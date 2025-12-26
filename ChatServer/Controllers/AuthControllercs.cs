using System.Security.Cryptography;
using System.Text;
using ChatServer.Data;
using ChatServer.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

// 회원가입 API
namespace ChatServer.Controllers
{
    [ApiController]
    [Route("auth")]

    public class AuthControllercs : ControllerBase
    {
        private readonly AppDbContext _db;

        // Program.cs 에서 사용한 키와 반드시 동일해야함!        
        private const string JwtKey = "이건_과제용_비밀키_아무문자나_길게";

        public AuthControllercs(AppDbContext db)
        {
            _db = db;
        }

        
        // 회원가입 API
        // POST /auth/signup        
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

        // 로그인 API
        // POST /auth/login
        [HttpPost("login")]
        public async Task<IActionResult> Login(string email, string password)
        {
            Console.WriteLine("로그인 요청이 들어왔습니다.");

            // 이메일로 사용자 조회
            var user = await _db.Users.FirstOrDefaultAsync(u => u.Email == email);

            if (user == null)
            {
                Console.WriteLine("존재하지 않는 이메일입니다.");
                return BadRequest("이메일이나 비밀번호가 올바르지 않습니다.");
            }

            // 이메일이 있다면 비밀번호 해시 비교
            var hashedPassword = HassPassword(password);

            if (user.PasswordHash != hashedPassword)
            {
                Console.WriteLine("비밀번호가 올바르지 않습니다.");
                return BadRequest("이메일이나 비밀번호가 올바르지 않습니다.");
            }

            // 비밀번호까지 맞다면 JWT 생성
            var token = GenerateJwtToken(user);

            Console.WriteLine("로그인 성공");

            // 토큰이랑 사용자 정보 반환
            return Ok(new
            {
                accessToken = token,
                user = new
                {
                    id = user.Id,
                    email = user.Email,
                    nickname = user.Nickname
                }
            });
        }
        
        // JWT 토큰 생성 함수        
        private string GenerateJwtToken(User user)
        {
            // JWT 안에 담을 정보(Claims)
            var claims = new[]
            {
                // 사용자 고유 ID
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),

                // 이메일
                new Claim(ClaimTypes.Email, user.Email)
            };

            // 서명 키 생성
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(JwtKey));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            // 토큰 생성
            var token = new JwtSecurityToken
            (
                claims: claims,
                expires: DateTime.Now.AddHours(1), // 1시간 유효
                signingCredentials: creds
            );

            // 문자열로 변환
            return new JwtSecurityTokenHandler().WriteToken(token);
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
