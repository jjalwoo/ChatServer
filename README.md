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

## 🎮 콘솔 클라이언트
Microsoft.AspNetCore.SignalR.Client

---

## 📁 프로젝트 구조

- ChatServer
  - Controllers
    - AuthController.cs (회원가입, 로그인)
    - UsersController.cs (내 정보 조회)
    - RoomsController.cs (채팅방 생성 및 조회)
  - Hubs
    - ChatHub.cs (SignalR 채팅 허브)
  - Models
    - User.cs
    - Room.cs
  - Data
    - AppDbContext.cs
  - Program.cs
  - appsettings.json


---

## 🔐 인증 구조 (JWT)

- 로그인 성공 시 서버에서 **JWT 토큰 발급**
- 인증이 필요한 API는 JWT 토큰을 통해 사용자 정보 식별
- SignalR(WebSocket) 연결 시에도 JWT 인증 필수
- SignalR 특성상 JWT 토큰은 `access_token` 쿼리 스트링으로 전달

---

## 📖 API 기능 설명

### 1. 회원가입
**POST** `/auth/signup`

### 요청
| 필드     | 설명                          |
|----------|-------------------------------|
| email    | 사용자 이메일 (중복 불가)     |
| password | 비밀번호 (서버에서 해시 처리) |
| nickname | 닉네임                        |

### 응답 예시
```json
{
  "userId": 1,
  "email": "test@test.com",
  "nickname": "tester",
  "createdAt": "2024-01-01T12:00:00"
}
```

### 예외 처리

- **이메일 중복** → `400 Bad Request`  
  메시지: `이미 사용 중인 이메일입니다.`

---

## 2. 로그인
**Post** `/auth/login`

**요청**  
| 필드 | 타입 | 설명 |
|------|------|------|
| email | string | 사용자 이메일 |
| password | string | 사용자 비밀번호 |

**응답 예시**  
```json
{
  "accessToken": "JWT_TOKEN",
  "user": {
    "id": 1,
    "email": "test@test.com",
    "nickname": "tester"
  }
}
```

### 예외 처리

- **이메일 또는 비밀번호 오류** → `400 Bad Request`  

---

## 3. 내 정보 조회(인증 필요)
**Get** `/users/me`

**헤더**
Authorization: Bearer JWT_TOKEN

**응답 예시**
```json
{
  "id": 1,
  "email": "test@test.com",
  "nickname": "tester",
  "createdAt": "2024-01-01T12:00:00"
}
```

### 예외 처리

- **토큰이 없거나 유효하지 않은 경우** → `401 Unauthorized`  
  
---

## 4. 채팅방 생성 (인증 필요)
**Post** `/rooms`

**설명**  
- 로그인한 사용자가 채팅방을 생성한다.  
- 생성자는 해당 채팅방의 **방장(Owner)** 으로 지정된다.

---
## 5. 채팅방 목록 조회 (인증 필요)
**GET** `/rooms`

**설명**  
- 사용자가 접근 가능한 채팅방 목록을 반환한다.
---
## 💬 실시간 채팅 (SignalR)

**Hub** `/chat`

**인증 방식**  
- JWT 필수 (`access_token` 사용)

**채팅 이벤트 흐름**  
1. SignalR 서버 연결 (JWT 검증)  
2. 채팅방 입장 → `JoinRoom(roomId)`  
3. 메시지 전송 → `SendMessage(roomId, message)`  
4. 메시지 수신 (브로드캐스트) → `ReceiveMessage(user, message)`
---
## 🖥 콘솔 클라이언트 테스트

**설명**  
- 콘솔 애플리케이션으로 SignalR 서버에 연결  
- JWT 토큰 입력 후 채팅방 ID 입력  
- 실시간 메시지 송수신 확인 가능  
- JWT 인증 실패 시 실패 로그 확인 가능  
- 콘솔 클라이언트는 테스트 용도입니다. 

---

### ▶ 실행 방법

1. MySQL 실행 및 데이터베이스 생성  
2. `appsettings.json`에 DB 연결 문자열 설정  
3. 서버 프로젝트 실행  
4. Swagger 접속 → `https://localhost:{port}/swagger`  
5. 회원가입 → 로그인 → JWT 발급  
6. API 또는 콘솔 클라이언트로 기능 테스트

