using System.Security.Claims;
using ChatServer.Data;
using ChatServer.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ChatServer.Controllers
{
    [ApiController]
    [Route("rooms")]
    [Authorize] // JWT 인증 필수
    public class RoomsController : ControllerBase
    {
        private readonly AppDbContext _db;

        public RoomsController(AppDbContext db)
        {
            _db = db;
        }

        // POST /rooms
        // 채팅방 생성
        [HttpPost]
        public async Task<IActionResult> CreateRoom(string name)
        {
            // JWT에서 사용자 ID 추출
            var userId = int.Parse( User.FindFirstValue(ClaimTypes.NameIdentifier)!);

            var room = new Room
            {
                Name = name,
                OwnerUserId = userId,
                CreatedAt = DateTime.UtcNow
            };

            _db.Rooms.Add(room);
            await _db.SaveChangesAsync();

            Console.WriteLine($"채팅방 '{name}'이(가) 생성되었습니다.");

            return Ok(new
            {
                roomId = room.Id,
                roomName = room.Name,
                createdAt = room.CreatedAt
            });
        }

        // GET /rooms
        // 채팅방 목록 조회
        // JWT 인증 필요
        [Authorize]
        [HttpGet]
        public async Task<IActionResult> GetRooms()
        {
            Console.WriteLine("채팅방 목록 조회 요청이 들어왔습니다.");

            // 모든 채팅방 조회
            var rooms = await _db.Rooms
                .OrderByDescending(r => r.CreatedAt)
                .Select(r => new
                {
                    roomId = r.Id,
                    name = r.Name,
                    ownerUserId = r.OwnerUserId,
                    createdAt = r.CreatedAt
                })
                .ToListAsync();

            Console.WriteLine("채팅방 목록 조회가 완료되었습니다.");

            return Ok(rooms);
        }
    }
}
