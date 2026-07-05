-- V2__seed_data.sql - SQLite Seed Data
-- Initial data: permissions, roles, admin user, configs

-- Permission data
INSERT INTO sys_permission (id, parent_id, name, code, type, path, component, icon, sort, visible, create_time) VALUES
(1, 0, 'System Management', 'system', 0, '/system', 'Layout', 'Setting', 1, 1, '2026-01-01 00:00:00'),
(2, 1, 'User Management', 'system:user', 0, '/system/users', 'system/user/index', 'User', 1, 1, '2026-01-01 00:00:00'),
(3, 1, 'Role Management', 'system:role', 0, '/system/roles', 'system/role/index', 'Avatar', 2, 1, '2026-01-01 00:00:00'),
(4, 1, 'Permission Management', 'system:permission', 0, '/system/permissions', 'system/permission/index', 'Lock', 3, 1, '2026-01-01 00:00:00'),
(5, 0, 'Log Management', 'logs', 0, '/logs', 'Layout', 'Document', 2, 1, '2026-01-01 00:00:00'),
(6, 5, 'Operation Log', 'logs:operation', 0, '/logs/operation', 'logs/operation/index', 'Edit', 1, 1, '2026-01-01 00:00:00'),
(7, 5, 'Exception Log', 'logs:exception', 0, '/logs/exception', 'logs/exception/index', 'Warning', 2, 1, '2026-01-01 00:00:00'),
(8, 0, 'Dashboard', 'dashboard', 0, '/dashboard', 'dashboard/index', 'HomeFilled', 0, 1, '2026-01-01 00:00:00'),
(9, 2, 'Query User', 'system:user:query', 1, NULL, NULL, NULL, 0, 0, '2026-01-01 00:00:00'),
(10, 2, 'Add User', 'system:user:add', 1, NULL, NULL, NULL, 1, 0, '2026-01-01 00:00:00'),
(11, 2, 'Edit User', 'system:user:edit', 1, NULL, NULL, NULL, 2, 0, '2026-01-01 00:00:00'),
(12, 2, 'Delete User', 'system:user:delete', 1, NULL, NULL, NULL, 3, 0, '2026-01-01 00:00:00'),
(13, 3, 'Query Role', 'system:role:query', 1, NULL, NULL, NULL, 0, 0, '2026-01-01 00:00:00'),
(14, 3, 'Add Role', 'system:role:add', 1, NULL, NULL, NULL, 1, 0, '2026-01-01 00:00:00'),
(15, 3, 'Edit Role', 'system:role:edit', 1, NULL, NULL, NULL, 2, 0, '2026-01-01 00:00:00'),
(16, 3, 'Delete Role', 'system:role:delete', 1, NULL, NULL, NULL, 3, 0, '2026-01-01 00:00:00'),
(17, 4, 'Query Permission', 'system:permission:query', 1, NULL, NULL, NULL, 0, 0, '2026-01-01 00:00:00'),
(18, 4, 'Add Permission', 'system:permission:add', 1, NULL, NULL, NULL, 1, 0, '2026-01-01 00:00:00'),
(19, 4, 'Edit Permission', 'system:permission:edit', 1, NULL, NULL, NULL, 2, 0, '2026-01-01 00:00:00'),
(20, 4, 'Delete Permission', 'system:permission:delete', 1, NULL, NULL, NULL, 3, 0, '2026-01-01 00:00:00'),
(21, 6, 'Query Operation Log', 'logs:operation:query', 1, NULL, NULL, NULL, 0, 0, '2026-01-01 00:00:00'),
(22, 6, 'Delete Operation Log', 'logs:operation:delete', 1, NULL, NULL, NULL, 1, 0, '2026-01-01 00:00:00'),
(23, 7, 'Query Exception Log', 'logs:exception:query', 1, NULL, NULL, NULL, 0, 0, '2026-01-01 00:00:00'),
(24, 7, 'Delete Exception Log', 'logs:exception:delete', 1, NULL, NULL, NULL, 1, 0, '2026-01-01 00:00:00'),
(25, 1, 'System Settings', 'system:config', 0, '/system/settings', 'system/settings/index', 'Tools', 4, 1, '2026-01-01 00:00:00'),
(26, 25, 'Query Settings', 'system:config:query', 1, NULL, NULL, NULL, 0, 0, '2026-01-01 00:00:00'),
(27, 25, 'Edit Settings', 'system:config:edit', 1, NULL, NULL, NULL, 1, 0, '2026-01-01 00:00:00');

-- Role data
INSERT INTO sys_role (id, name, code, description, create_time) VALUES
(1, 'Super Admin', 'super_admin', 'System super administrator with all permissions', '2026-01-01 00:00:00');

-- Admin user (password: admin123, BCrypt hash)
INSERT INTO sys_user (id, username, password, nickname, email, status, is_deleted, create_time) VALUES
(1, 'admin', '$2a$10$5iAHZMPe1V7GZv1k1y/dDuglipsuiyFr4PEKI2xeOn93Dle2GgK2u', 'Super Admin', 'admin@zjadmin.com', 1, 0, '2026-01-01 00:00:00');

-- User role association
INSERT INTO sys_user_role (id, user_id, role_id) VALUES
(1, 1, 1);

-- Role permission association
INSERT INTO sys_role_permission (id, role_id, permission_id) VALUES
(1, 1, 1), (2, 1, 2), (3, 1, 3), (4, 1, 4), (5, 1, 5),
(6, 1, 6), (7, 1, 7), (8, 1, 8), (9, 1, 9), (10, 1, 10),
(11, 1, 11), (12, 1, 12), (13, 1, 13), (14, 1, 14), (15, 1, 15),
(16, 1, 16), (17, 1, 17), (18, 1, 18), (19, 1, 19), (20, 1, 20),
(21, 1, 21), (22, 1, 22), (23, 1, 23), (24, 1, 24), (25, 1, 25),
(26, 1, 26), (27, 1, 27);

-- System configs
INSERT INTO sys_config (id, config_key, config_value, description, create_time) VALUES
(1, 'site_title', 'ZJAdmin', 'Site title', '2026-01-01 00:00:00'),
(2, 'site_keywords', 'ZJAdmin,Backend,Admin,RBAC', 'Site keywords', '2026-01-01 00:00:00'),
(3, 'site_icp', '', 'ICP info', '2026-01-01 00:00:00'),
(4, 'site_copyright', 'Copyright 2026 ZJAdmin. All rights reserved.', 'Copyright info', '2026-01-01 00:00:00');