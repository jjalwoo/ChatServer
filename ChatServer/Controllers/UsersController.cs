using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ChatServer.Data;
using System.Security.Claims;

namespace ChatServer.Controllers
{
    [ApiController]
    [Route("users")]
    public class UsersController : ControllerBase
    {
        private readonly AppDbContext _db;

        public UsersController(AppDbContext db)
        {
            _db = db;
        }

        // 내 정보 조회 API
        // GET /users/me
        // JWT 인증 필수
        [Authorize]
        [HttpGet("me")]
        public IActionResult GetMyInfo()
        {
            Console.WriteLine("내 정보 조회 요청이 들어왔습니다.");

            // JWT 토큰에서 userId 추출
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);

            if (userIdClaim == null)
            {
                Console.WriteLine("토큰에서 사용자 정보를 찾을 수 없습니다.");
                return Unauthorized();
            }

            var userId = int.Parse(userIdClaim.Value);

            // DB에서 사용자 조회
            var user = _db.Users.Find(userId);

            if (user == null)
            {
                Console.WriteLine("사용자를 찾을 수 없습니다.");
                return NotFound();
            }

            return Ok(new
            {
                id = user.Id,
                email = user.Email,
                nickname = user.Nickname,
                createdAt = user.CreatedAt
            });
        }
    }
}
