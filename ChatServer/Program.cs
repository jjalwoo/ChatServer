using ChatServer.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);


// DB 설정 (기존 코드 그대로)
builder.Services.AddDbContext<AppDbContext>(options =>
{
    options.UseMySql(
        builder.Configuration.GetConnectionString("DefaultConnection"),
        ServerVersion.AutoDetect(builder.Configuration.GetConnectionString("DefaultConnection"))
    );
});


// JWT 비밀키
var jwtKey = "이건_과제용_비밀키_아무문자나_길게";


// JWT 인증 설정
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        // 서명 검증
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(jwtKey)
        ),

        // 과제에서는 발급자/대상 검증 생략
        ValidateIssuer = false,
        ValidateAudience = false,

        // 만료 시간 검증
        ValidateLifetime = true,
        ClockSkew = TimeSpan.Zero
    };

    options.Events = new Microsoft.AspNetCore.Authentication.JwtBearer.JwtBearerEvents
    {
        OnMessageReceived = context =>
        {
            // 들어온 Authorization 헤더를 콘솔에 찍음 (디버그용)
            var auth = context.Request.Headers["Authorization"].FirstOrDefault();
            Console.WriteLine($"[JWT] OnMessageReceived Authorization header: {auth}");
            return Task.CompletedTask;
        },
        OnTokenValidated = context =>
        {
            Console.WriteLine("[JWT] OnTokenValidated: 토큰 검증 성공");
            return Task.CompletedTask;
        },
        OnAuthenticationFailed = context =>
        {
            // 검증 실패 이유를 콘솔에 찍음 ? 반드시 출력값을 복사해서 보내줘
            Console.WriteLine("[JWT] OnAuthenticationFailed: " + context.Exception?.ToString());
            return Task.CompletedTask;
        }
    };
});


// 컨트롤러 / Swagger
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
    {
        Title = "ChatServer API",
        Version = "v1"
    });

    // JWT 인증 정의
    options.AddSecurityDefinition("Bearer", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = Microsoft.OpenApi.Models.SecuritySchemeType.Http,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = Microsoft.OpenApi.Models.ParameterLocation.Header,
        Description = "Bearer {토큰} 형식으로 입력하세요"
    });

    // JWT 인증 요구
    options.AddSecurityRequirement(new Microsoft.OpenApi.Models.OpenApiSecurityRequirement
    {
        {
            new Microsoft.OpenApi.Models.OpenApiSecurityScheme
            {
                Reference = new Microsoft.OpenApi.Models.OpenApiReference
                {
                    Type = Microsoft.OpenApi.Models.ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new string[] {}
        }
    });
});
var app = builder.Build();

// 미들웨어
app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();


app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
