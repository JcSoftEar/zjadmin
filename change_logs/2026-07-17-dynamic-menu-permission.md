# 动态菜单权限加载

## 概述
除仪表盘和个人中心外，所有菜单根据权限管理配置动态加载。

## 修改内容

### 后端 - C# (backend\ZJAdmin.Api)
- **AuthService.cs**: 新增 `GetCurrentUserMenus(long userId)` 方法，根据用户权限返回菜单树（仅 type=0, visible=1 的菜单项）
  - 超级管理员返回全部菜单
  - 普通用户返回有权限的菜单及其父级路径
  - 权限匹配规则：拥有菜单 code 或任意子权限 code 即可访问该菜单
- **AuthController.cs**: 新增 `GET /api/v1/auth/menus` 端点

### 后端 - Java (backend-java)
- **AuthService.java**: 接口新增 `getCurrentUserMenus(Long userId)` 方法
- **AuthServiceImpl.java**: 实现菜单树构建逻辑，与 C# 版本一致
- **AuthController.java**: 新增 `GET /api/v1/auth/menus` 端点

### 前端 (frontend)
- **api/auth.js**: 新增 `getUserMenus()` API 调用
- **stores/auth.js**: 新增 `menus` 状态和 `fetchMenus()` 异步操作
- **layouts/Sidebar.vue**: 重写为动态菜单结构
  - 首页、个人中心保持静态始终显示
  - 中间菜单从 `authStore.menus` 动态渲染
- **layouts/SidebarMenuItem.vue**: 新增递归组件，支持无限层级菜单嵌套
- **layouts/AppLayout.vue**: 挂载时调用 `fetchMenus()`

## 工作原理
1. 用户登录后，`AppLayout` 调用 `fetchProfile()` 和 `fetchMenus()`
2. 后端 `GET /api/v1/auth/menus` 根据用户角色权限计算可见菜单树
3. 前端 Sidebar 将静态菜单（首页、个人中心）与动态菜单合并渲染
4. 动态菜单自动跳过 `/dashboard` 避免与静态首页重复
