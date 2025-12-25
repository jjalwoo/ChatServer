using System;

// 사용자 정보를 DB 테이블로 만들기 위한 클래스
namespace ChatServer.Models
{
    public class User
    {
        // PK (자동증가)
        public int Id { get; set; }

        // 이메일
        public string Email { get; set; }

        // 비밀번호 해시
        public string PasswordHash { get; set; }

        // 닉네임
        public string Nickname{ get; set; }

        // 생성시간
        public DateTime CreatedAt { get; set; }
    }
}
