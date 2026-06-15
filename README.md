# ZJAdmin

> [English](README.en.md) | **中文**

基于 **ASP.NET Core 8** + **Vue 3** + **Element Plus** 的轻量级 RBAC 后台管理模板。

## 截图

| 登录页 | 仪表盘 |
:---:|:---:
| ![登录](images/zj-1.jpg) | ![仪表盘](images/zj-2.jpg) |

| 用户管理 | 角色管理 | 权限管理 |
:---:|:---:|:---:
| ![用户](images/zj-3.jpg) | ![角色](images/zj-4.jpg) | ![权限](images/zj-5.jpg) |

## 功能特性

- **RBAC 权限管理** — 基于角色的访问控制，支持按钮级细粒度权限
- **多数据库支持** — 默认 SQLite，切换配置文件即可使用 MySQL
- **JWT 认证** — 基于 Token 的身份认证，带登录锁定保护
- **操作日志** — 自动记录每次 API 请求的审计轨迹
- **异常日志** — 服务端异常与登录失败记录持久化
- **菜单管理** — 根据权限数据动态生成菜单和按钮
- **系统配置** — 站点标题、关键词、备案号、版权等键值配置

## 技术栈

| 层次 | 技术 |
|------|------|
| 后端 | ASP.NET Core 8 Web API |
| ORM | Entity Framework Core 8 |
| 前端 | Vue 3 (Composition API) + Vite |
| UI | Element Plus |
| 认证 | JWT Bearer + BCrypt |
| 日志 | Serilog |
| 数据库 | SQLite / MySQL |

## 快速开始

### 环境要求

- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- [Node.js 18+](https://nodejs.org/)

### 启动后端

```bash
cd backend/ZJAdmin.Api
dotnet run --urls "http://localhost:5000"
```

首次运行会自动创建数据库并初始化种子数据。

### 启动前端

```bash
cd frontend
npm install
npm run dev
```

开发服务器运行在 `http://localhost:3417`，已配置将 `/api` 代理到后端。

### 默认登录账号

| 用户名 | 密码 | 角色 |
|--------|------|------|
| `admin` | `admin123` | 超级管理员（全部权限） |

## 数据库

默认使用 **SQLite**，无需额外配置。

如需切换到 **MySQL**，编辑 `backend/ZJAdmin.Api/appsettings.json`：

```json
{
  "DatabaseProvider": "Mysql",
  "ConnectionStrings": {
    "MySqlConnection": "Server=your_host;Port=3306;Database=zjadmin;User=your_user;Password=your_pass;Charset=utf8mb4"
  }
}
```

表结构会在首次运行时自动创建。

## Docker

```bash
docker compose up -d
```

应用通过 Nginx 在 `8080` 端口提供服务。可通过环境变量覆盖数据库配置：

```bash
DATABASE_PROVIDER=MySql MYSQL_CONNECTION_STRING="..." docker compose up -d
```

## API

所有接口遵循统一的响应格式：

```json
{
  "code": 200,
  "message": "success",
  "data": {},
  "total": 100
}
```

| 模块 | 路径 | 说明 |
|------|------|------|
| 认证 | `/api/v1/auth/*` | 登录、退出、个人信息、修改密码 |
| 用户 | `/api/v1/users/*` | 用户增删改查、重置密码、启用/禁用 |
| 角色 | `/api/v1/roles/*` | 角色增删改查、分配权限 |
| 权限 | `/api/v1/permissions/*` | 权限增删改查、权限树 |
| 操作日志 | `/api/v1/operation-logs/*` | 查询、详情、导出、清理 |
| 异常日志 | `/api/v1/exception-logs/*` | 查询、详情、导出、清理 |

## 项目结构

```
├── backend/
│   └── ZJAdmin.Api/
│       ├── Controllers/    # API 控制器
│       ├── Services/       # 业务逻辑层
│       ├── Models/         # 实体模型
│       ├── DTOs/           # 数据传输对象
│       ├── Data/           # DbContext 与种子数据
│       ├── Middleware/     # 异常处理、操作日志、权限认证中间件
│       ├── Attributes/     # 自定义特性
│       └── Program.cs      # 启动配置
├── frontend/
│   └── src/
│       ├── api/            # Axios API 层
│       ├── layouts/        # 布局组件
│       ├── views/          # 页面组件
│       ├── stores/         # Pinia 状态管理（auth, app）
│       ├── router/         # Vue Router
│       └── utils/          # 工具函数
├── images/                 # 截图
├── docker-compose.yml
└── Dockerfile
```

## License

MIT
