using System;
using System.Data;

namespace ChatServer.Models
{
    public class Room
    {
        public int Id { get; set; }

        // 채팅방 이름
        public string Name { get; set; } = null;

        // 방장 아이디
        public int OwnerUserId { get; set; }

        // 생성 시간
        public DateTime CreatedAt { get; set; }

    }
}
