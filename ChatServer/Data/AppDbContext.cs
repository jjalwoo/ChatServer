using Microsoft.EntityFrameworkCore;
using ChatServer.Models;

namespace ChatServer.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }

        // table을 생성
        // User 클래스를 기반으로 Users라는 테이블을 만든다.
        public DbSet<User> Users { get; set; }
    }
}
