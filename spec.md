# 最简后台 (zjadmin) 技术规格文档

## 一、项目概述

基于 .NET Core 8 + Vue 3 的极简 RBAC 后台管理系统。

## 二、项目结构

### 2.1 整体结构

```
F:\Projects\JCsoft\最简后台\
├── backend/                    # .NET Core 8 Web API 后端
│   ├── ZJAdmin.Api/            # 主项目
│   │   ├── Controllers/        # API 控制器
│   │   ├── Models/             # 实体模型
│   │   ├── DTOs/               # 数据传输对象
│   │   ├── Services/           # 业务逻辑层
│   │   ├── Middleware/         # 中间件
│   │   ├── Data/               # DbContext + 种子数据
│   │   ├── Attributes/         # 自定义特性
│   │   └── Program.cs
│   └── ZJAdmin.Api.sln
├── frontend/                   # Vue 3 前端
│   ├── src/
│   │   ├── api/                # API 接口层
│   │   ├── assets/             # 静态资源
│   │   ├── components/         # 公共组件
│   │   ├── layouts/            # 布局组件
│   │   ├── router/             # 路由配置
│   │   ├── stores/             # Pinia 状态管理
│   │   ├── views/              # 页面视图
│   │   ├── utils/              # 工具函数
│   │   ├── App.vue
│   │   └── main.js
│   ├── package.json
│   └── vite.config.js
├── spec.md                     # 本技术规格文档
└── 最简后台(zjadmin)产品需求文档(PRD).md
```

### 2.2 后端分层架构

```
Controller (API 入口) → Service (业务逻辑) → EF Core (数据访问)
                                               ↕
                                          SQLite
```

## 三、数据库设计

### 3.1 完整表结构

见 PRD 第六章，使用 SQLite 数据库，默认文件 `ZJAdmin.db`。

### 3.2 种子数据

| 用户名 | 密码 | 角色 | 说明 |
|--------|------|------|------|
| admin | admin123 | 超级管理员 | 系统内置超级管理员 |

- 超级管理员角色：不可删除，code 为 `super_admin`
- 默认权限：系统所有菜单和按钮权限

## 四、后端 API 设计

### 4.1 通用约定

- 基础路径：`/api/v1`
- 响应格式：
```json
{
  "code": 200,         // 状态码
  "message": "成功",   // 提示信息
  "data": {},          // 响应数据
  "total": 100         // 分页总数（仅列表接口）
}
```
- 状态码：200 成功，400 参数错误，401 未认证，403 无权限，404 未找到，500 服务器错误
- 分页参数：`page`(页码, 默认1), `pageSize`(每页条数, 默认10)

### 4.2 接口清单

#### 认证模块 `/api/v1/auth`

| 方法 | 路径 | 说明 | 认证 |
|------|------|------|------|
| POST | /login | 用户登录 | 否 |
| POST | /logout | 退出登录 | 是 |
| GET  | /profile | 获取当前用户信息 | 是 |
| PUT  | /profile | 修改个人信息 | 是 |
| PUT  | /password | 修改密码 | 是 |

#### 用户管理 `/api/v1/users`

| 方法 | 路径 | 说明 | 认证 |
|------|------|------|------|
| GET  | / | 分页查询用户列表 | 是 |
| POST | / | 新增用户 | 是 |
| GET  | /{id} | 获取用户详情 | 是 |
| PUT  | /{id} | 编辑用户 | 是 |
| DELETE | /{id} | 删除用户 | 是 |
| PUT  | /{id}/reset-password | 重置密码 | 是 |
| PUT  | /{id}/status | 启用/禁用用户 | 是 |

#### 角色管理 `/api/v1/roles`

| 方法 | 路径 | 说明 | 认证 |
|------|------|------|------|
| GET  | / | 分页查询角色列表 | 是 |
| GET  | /all | 查询所有角色（无分页） | 是 |
| POST | / | 新增角色 | 是 |
| GET  | /{id} | 获取角色详情 | 是 |
| PUT  | /{id} | 编辑角色 | 是 |
| DELETE | /{id} | 删除角色 | 是 |
| PUT  | /{id}/permissions | 分配权限 | 是 |
| GET  | /{id}/permissions | 获取角色的权限ID列表 | 是 |

#### 权限管理 `/api/v1/permissions`

| 方法 | 路径 | 说明 | 认证 |
|------|------|------|------|
| GET  | /tree | 获取权限树 | 是 |
| POST | / | 新增权限 | 是 |
| GET  | /{id} | 获取权限详情 | 是 |
| PUT  | /{id} | 编辑权限 | 是 |
| DELETE | /{id} | 删除权限 | 是 |

#### 操作日志 `/api/v1/operation-logs`

| 方法 | 路径 | 说明 | 认证 |
|------|------|------|------|
| GET  | / | 分页查询操作日志 | 是 |
| GET  | /{id} | 获取日志详情 | 是 |
| DELETE | /clean | 清理操作日志 | 是 |
| GET  | /export | 导出操作日志 Excel | 是 |

#### 异常日志 `/api/v1/exception-logs`

| 方法 | 路径 | 说明 | 认证 |
|------|------|------|------|
| GET  | / | 分页查询异常日志 | 是 |
| GET  | /{id} | 获取异常日志详情 | 是 |
| DELETE | /clean | 清理异常日志 | 是 |
| GET  | /export | 导出异常日志 Excel | 是 |

### 4.3 登录接口详细设计

**POST /api/v1/auth/login**

请求：
```json
{
  "username": "admin",
  "password": "admin123"
}
```

响应：
```json
{
  "code": 200,
  "message": "登录成功",
  "data": {
    "token": "eyJhbGciOiJIUzI1NiIs...",
    "user": {
      "id": 1,
      "username": "admin",
      "nickname": "超级管理员",
      "email": "admin@zjadmin.com",
      "phone": "",
      "avatar": "",
      "roles": ["super_admin"],
      "permissions": ["*:*:*"]
    }
  }
}
```

### 4.4 JWT 认证

- Token 有效期：2 小时
- Token 中携带：userId, username, roles, permissions
- 通过自定义 `[Permission]` 特性标记需要权限的接口
- 登录失败 5 次锁定账户 15 分钟（内存缓存）

## 五、前端路由设计

### 5.1 路由结构

```
/login                    # 登录页（无需认证）
/                         # 首页布局（需认证）
├── /dashboard            # 首页/仪表盘
├── /system               # 系统管理
│   ├── /system/users     # 用户管理
│   ├── /system/roles     # 角色管理
│   └── /system/permissions # 权限管理
├── /logs                 # 日志管理
│   ├── /logs/operation   # 操作日志
│   └── /logs/exception   # 异常日志
└── /profile              # 个人中心
```

### 5.2 路由守卫

- 未登录 → 重定向到 /login
- 无权限 → 显示 403 页面
- 动态路由：根据用户权限动态生成侧边栏菜单

## 六、前端组件设计

### 6.1 布局组件

- `AppLayout.vue` - 整体布局（顶部导航 + 左侧菜单 + 主内容区）
- `Sidebar.vue` - 左侧侧边栏（根据权限动态生成菜单树）
- `Navbar.vue` - 顶部导航栏（用户信息、退出登录）

### 6.2 公共组件

- `PaginationTable.vue` - 封装 Element Plus 表格 + 分页
- `SearchForm.vue` - 搜索表单组件
- `TreeDialog.vue` - 权限树弹窗组件

### 6.3 页面视图

每个模块由以下文件组成：
- `index.vue` - 列表页（查询、分页、操作按钮）
- `FormDialog.vue` - 新增/编辑表单弹窗
- `DetailDialog.vue` - 详情查看弹窗（日志模块）

## 七、开发步骤

### 阶段一：后端基础搭建（步骤 1-3）

1. 创建 .NET Core 8 Web API 项目，配置 Swagger、JWT、EF Core + SQLite
2. 创建数据模型和 DbContext，配置种子数据
3. 实现认证模块（登录、JWT 中间件、当前用户）

### 阶段二：后端业务功能（步骤 4-9）

4. 用户管理 CRUD API
5. 角色管理 CRUD API + 权限分配
6. 权限管理 CRUD API + 权限树
7. 操作日志 API + 自动记录中间件
8. 异常日志 API + 全局异常处理
9. 个人中心 API

### 阶段三：前端基础搭建（步骤 10-12）

10. 创建 Vue 3 + Vite 项目，配置 Element Plus、Pinia、Vue Router
11. 实现布局组件（Layout、Sidebar、Navbar）
12. 实现登录页、路由守卫、Token 管理

### 阶段四：前端业务功能（步骤 13-18）

13. 首页仪表盘
14. 用户管理页面
15. 角色管理页面
16. 权限管理页面（树形结构）
17. 日志管理页面（操作日志 + 异常日志）
18. 个人中心页面

### 阶段五：完善（步骤 19-20）

19. 权限指令 v-permission 实现前端按钮级权限控制
20. 联调测试，修复问题
