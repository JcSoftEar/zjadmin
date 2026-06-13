# ZJAdmin 最简后台

一套极简、轻量、开箱即用的通用后台管理系统，基于 **.NET Core 8** + **Vue 3** 构建，仅保留企业后台最核心的 RBAC 权限体系和操作日志功能。

## 特性

- **RBAC 权限管理** — 用户-角色-权限三级权限模型，支持菜单权限和按钮级权限控制
- **操作日志** — 自动记录所有用户操作，支持详情查看和 Excel 导出
- **异常日志** — 自动捕获系统异常，记录完整堆栈信息
- **JWT 认证** — 基于 Token 的无状态认证，支持 2 小时有效期
- **动态菜单** — 根据用户权限动态生成侧边栏菜单
- **权限指令** — 前端 `v-permission` 指令实现按钮级权限控制
- **登录保护** — 登录失败 5 次锁定 15 分钟，密码 BCrypt 加密
- **开箱即用** — 默认 SQLite 数据库，无需额外配置即可启动

## 技术栈

| 层级 | 技术 |
|------|------|
| 后端框架 | ASP.NET Core 8 Web API |
| ORM | Entity Framework Core 8 |
| 数据库 | SQLite (支持 SQL Server / MySQL) |
| 身份认证 | JWT (JSON Web Token) |
| 日志 | Serilog |
| 前端框架 | Vue 3 (组合式 API) |
| 构建工具 | Vite |
| UI 组件库 | Element Plus |
| 状态管理 | Pinia |
| HTTP 请求 | Axios |

## 快速开始

### 环境要求

- .NET 8 SDK
- Node.js 18+

### 启动后端

```bash
cd backend/ZJAdmin.Api
dotnet run --urls "http://localhost:5000"
```

首次运行会自动创建 SQLite 数据库并初始化种子数据。

### 启动前端

```bash
cd frontend
npm install
npm run dev
```

前端开发服务器默认运行在 `http://localhost:3417`，已配置 API 代理到后端 `http://localhost:5000`。

### 默认账户

| 用户名 | 密码 | 角色 |
|--------|------|------|
| admin | admin123 | 超级管理员（拥有所有权限） |

## 项目结构

```
├── backend/                      # .NET Core 8 后端
│   └── ZJAdmin.Api/
│       ├── Controllers/          # API 控制器
│       ├── Models/               # 实体模型
│       ├── DTOs/                 # 数据传输对象
│       ├── Services/             # 业务逻辑层
│       ├── Middleware/           # 中间件（异常处理、操作日志、权限过滤）
│       ├── Data/                 # DbContext + 种子数据
│       ├── Attributes/           # 自定义特性（权限标注）
│       └── Program.cs            # 启动配置
├── frontend/                     # Vue 3 前端
│   └── src/
│       ├── api/                  # API 接口层
│       ├── layouts/              # 布局组件
│       ├── views/                # 页面视图
│       ├── stores/               # Pinia 状态管理
│       ├── router/               # 路由配置
│       └── utils/                # 工具函数
├── spec.md                       # 技术规格文档
└── README.md                     # 项目说明
```

## API 接口

所有 API 接口遵循统一响应格式：

```json
{
  "code": 200,
  "message": "成功",
  "data": {},
  "total": 100
}
```

### 主要接口

| 模块 | 路径 | 说明 |
|------|------|------|
| 认证 | `/api/v1/auth/*` | 登录、退出、个人信息、修改密码 |
| 用户 | `/api/v1/users/*` | 用户 CRUD、重置密码、启用/禁用 |
| 角色 | `/api/v1/roles/*` | 角色 CRUD、分配权限 |
| 权限 | `/api/v1/permissions/*` | 权限 CRUD、权限树 |
| 操作日志 | `/api/v1/operation-logs/*` | 日志查询、详情、导出、清理 |
| 异常日志 | `/api/v1/exception-logs/*` | 日志查询、详情、导出、清理 |

## 许可证

MIT
