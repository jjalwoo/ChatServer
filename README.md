# 💬 Chat Server (ASP.NET Core · JWT · SignalR)

.NET 8 기반의 **로그인 / 회원가입 / JWT 인증 / 실시간 채팅 서버**입니다.  
REST API와 SignalR(WebSocket 기반)을 함께 사용하여 **인증된 사용자만 채팅에 참여**할 수 있도록 구현했습니다.

---

## 📌 프로젝트 개요

본 프로젝트는 다음 기능을 제공합니다.

- 회원가입 / 로그인
- JWT 기반 인증 및 인가
- 인증된 사용자 정보 조회
- 채팅방 생성 및 조회
- SignalR 기반 실시간 채팅
- Swagger(OpenAPI)를 통한 API 문서화
- 콘솔 기반 채팅 클라이언트 테스트

---

## 🛠 기술 스택

### Backend
- **.NET 8 (ASP.NET Core)**
- **Entity Framework Core**
- **MySQL**
- **SignalR**

### 인증
- **JWT (JSON Web Token)**
- Bearer Token Authentication

### 문서화
- **Swagger (OpenAPI)**

---

## 📦 NuGet 패키지

### 서버 프로젝트
Microsoft.EntityFrameworkCore
Microsoft.EntityFrameworkCore.Tools
Pomelo.EntityFrameworkCore.MySql

Microsoft.AspNetCore.Authentication.JwtBearer
System.IdentityModel.Tokens.Jwt

Microsoft.AspNetCore.SignalR

## 📦 NuGet 패키지

### 콘솔 클라이언트
- Microsoft.AspNetCore.SignalR.Client

---

## 🗂 프로젝트 구조

ChatServer
 ├─ Controllers
 │   ├─ AuthController.cs      // 회원가입, 로그인
 │   ├─ UsersController.cs     // 내 정보 조회
 │   ├─ RoomsController.cs     // 채팅방 생성 및 조회
 │
 ├─ Hubs
 │   └─ ChatHub.cs             // SignalR 채팅 허브
 │
 ├─ Models
 │   ├─ User.cs
 │   └─ Room.cs
 │
 ├─ Data
 │   └─ AppDbContext.cs
 │
 ├─ Program.cs
 └─ appsettings.json
