using System;
using System.Security.Claims;
using ChatServer.Data;
using ChatServer.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore; // Hub 사용

namespace ChatServer.Hubs
{
    // JWT 인증 필수
    [Authorize]
    public class ChatHub : Hub
    {
        private readonly AppDbContext _db;

        public ChatHub(AppDbContext db)
        {
            _db = db;
        }


        // 허브에 연결될 때 호출
        // JWT가 유효하지 않으면 여기까지 못 옴         
        public override async Task OnConnectedAsync()
        {
            // JWT에서 userId 추출
            var userId = Context.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            Console.WriteLine($"[SignalR] 사용자 연결됨 (UserId: {userId})");

            await base.OnConnectedAsync();
        }

        
        // 채팅방 입장
        // SignalR의 Group 기능 사용        
        public async Task JoinRoom(int roomId)
        {
            // 방 존재 여부 확인
            var roomExists = await _db.Rooms.AnyAsync(r => r.Id == roomId);

            if (!roomExists)
            {
                // 클라이언트에게 오류 전달
                await Clients.Caller.SendAsync( "ReceiveMessage", "SYSTEM", "존재하지 않는 채팅방입니다.");
                return;
            }

            // 연결 ID를 roomId 그룹에 추가
            await Groups.AddToGroupAsync(Context.ConnectionId, roomId.ToString());

            Console.WriteLine($"[SignalR] 채팅방 입장 (RoomId: {roomId})");

            // 방에 입장 알림 브로드캐스트
            await Clients.Group(roomId.ToString()).SendAsync("ReceiveMessage", "SYSTEM", $"사용자가 채팅방 {roomId}에 입장했습니다.");
        }

        
        // 메시지 전송         
        public async Task SendMessage(int roomId, string message)
        {
            var nickname = Context.User?.FindFirst(ClaimTypes.Email)?.Value ?? "알 수 없음";

            Console.WriteLine($"[SignalR] 보낸 사람: {nickname}, 내용: {message}");

            // 같은 방 사용자들에게 메시지 전달
            await Clients.Group(roomId.ToString()).SendAsync("ReceiveMessage", nickname, message);
        }

        // 연결 종료 시 호출
        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            Console.WriteLine("[SignalR] 사용자 연결 종료됨");
            await base.OnDisconnectedAsync(exception);
        }
    }
}
