-- V2__seed_data.sql - SQLite Seed Data
-- 初始数据：权限、角色、管理员用户、配置

-- 权限数据
INSERT INTO sys_permission (id, parent_id, name, code, type, path, component, icon, sort, visible, create_time) VALUES
(1, 0, '系统管理', 'system', 0, '/system', 'Layout', 'Setting', 1, 1, '2026-01-01 00:00:00'),
(2, 1, '用户管理', 'system:user', 0, '/system/users', 'system/user/index', 'User', 1, 1, '2026-01-01 00:00:00'),
(3, 1, '角色管理', 'system:role', 0, '/system/roles', 'system/role/index', 'Avatar', 2, 1, '2026-01-01 00:00:00'),
(4, 1, '权限管理', 'system:permission', 0, '/system/permissions', 'system/permission/index', 'Lock', 3, 1, '2026-01-01 00:00:00'),
(5, 0, '日志管理', 'logs', 0, '/logs', 'Layout', 'Document', 2, 1, '2026-01-01 00:00:00'),
(6, 5, '操作日志', 'logs:operation', 0, '/logs/operation', 'logs/operation/index', 'Edit', 1, 1, '2026-01-01 00:00:00'),
(7, 5, '异常日志', 'logs:exception', 0, '/logs/exception', 'logs/exception/index', 'Warning', 2, 1, '2026-01-01 00:00:00'),
(8, 0, '首页', 'dashboard', 0, '/dashboard', 'dashboard/index', 'HomeFilled', 0, 1, '2026-01-01 00:00:00'),
(9, 2, '查询用户', 'system:user:query', 1, NULL, NULL, NULL, 0, 0, '2026-01-01 00:00:00'),
(10, 2, '新增用户', 'system:user:add', 1, NULL, NULL, NULL, 1, 0, '2026-01-01 00:00:00'),
(11, 2, '编辑用户', 'system:user:edit', 1, NULL, NULL, NULL, 2, 0, '2026-01-01 00:00:00'),
(12, 2, '删除用户', 'system:user:delete', 1, NULL, NULL, NULL, 3, 0, '2026-01-01 00:00:00'),
(13, 3, '查询角色', 'system:role:query', 1, NULL, NULL, NULL, 0, 0, '2026-01-01 00:00:00'),
(14, 3, '新增角色', 'system:role:add', 1, NULL, NULL, NULL, 1, 0, '2026-01-01 00:00:00'),
(15, 3, '编辑角色', 'system:role:edit', 1, NULL, NULL, NULL, 2, 0, '2026-01-01 00:00:00'),
(16, 3, '删除角色', 'system:role:delete', 1, NULL, NULL, NULL, 3, 0, '2026-01-01 00:00:00'),
(17, 4, '查询权限', 'system:permission:query', 1, NULL, NULL, NULL, 0, 0, '2026-01-01 00:00:00'),
(18, 4, '新增权限', 'system:permission:add', 1, NULL, NULL, NULL, 1, 0, '2026-01-01 00:00:00'),
(19, 4, '编辑权限', 'system:permission:edit', 1, NULL, NULL, NULL, 2, 0, '2026-01-01 00:00:00'),
(20, 4, '删除权限', 'system:permission:delete', 1, NULL, NULL, NULL, 3, 0, '2026-01-01 00:00:00'),
(21, 6, '查询操作日志', 'logs:operation:query', 1, NULL, NULL, NULL, 0, 0, '2026-01-01 00:00:00'),
(22, 6, '删除操作日志', 'logs:operation:delete', 1, NULL, NULL, NULL, 1, 0, '2026-01-01 00:00:00'),
(23, 7, '查询异常日志', 'logs:exception:query', 1, NULL, NULL, NULL, 0, 0, '2026-01-01 00:00:00'),
(24, 7, '删除异常日志', 'logs:exception:delete', 1, NULL, NULL, NULL, 1, 0, '2026-01-01 00:00:00'),
(25, 1, '系统设置', 'system:config', 0, '/system/settings', 'system/settings/index', 'Tools', 4, 1, '2026-01-01 00:00:00'),
(26, 25, '查询系统设置', 'system:config:query', 1, NULL, NULL, NULL, 0, 0, '2026-01-01 00:00:00'),
(27, 25, '编辑系统设置', 'system:config:edit', 1, NULL, NULL, NULL, 1, 0, '2026-01-01 00:00:00');

-- 角色数据
INSERT INTO sys_role (id, name, code, description, create_time) VALUES
(1, '超级管理员', 'super_admin', '系统超级管理员，拥有所有权限', '2026-01-01 00:00:00');

-- 管理员用户 (密码: admin123, BCrypt hash)
INSERT INTO sys_user (id, username, password, nickname, email, status, is_deleted, create_time) VALUES
(1, 'admin', '$2a$10$N.zmdr9k7uOCQb376NoUnuTJ8iAt6Z5EHsM8lE9lBOsl7iAt6Z5EH', '超级管理员', 'admin@zjadmin.com', 1, 0, '2026-01-01 00:00:00');

-- 用户角色关联
INSERT INTO sys_user_role (id, user_id, role_id) VALUES
(1, 1, 1);

-- 角色权限关联
INSERT INTO sys_role_permission (id, role_id, permission_id) VALUES
(1, 1, 1), (2, 1, 2), (3, 1, 3), (4, 1, 4), (5, 1, 5),
(6, 1, 6), (7, 1, 7), (8, 1, 8), (9, 1, 9), (10, 1, 10),
(11, 1, 11), (12, 1, 12), (13, 1, 13), (14, 1, 14), (15, 1, 15),
(16, 1, 16), (17, 1, 17), (18, 1, 18), (19, 1, 19), (20, 1, 20),
(21, 1, 21), (22, 1, 22), (23, 1, 23), (24, 1, 24), (25, 1, 25),
(26, 1, 26), (27, 1, 27);

-- 系统配置
INSERT INTO sys_config (id, config_key, config_value, description, create_time) VALUES
(1, 'site_title', 'ZJAdmin 最简后台', '网站标题', '2026-01-01 00:00:00'),
(2, 'site_keywords', 'ZJAdmin,最简后台,后台管理,RBAC', '网站关键词', '2026-01-01 00:00:00'),
(3, 'site_icp', '', '备案信息', '2026-01-01 00:00:00'),
(4, 'site_copyright', 'Copyright © 2026 ZJAdmin. All rights reserved.', '版权信息', '2026-01-01 00:00:00');
