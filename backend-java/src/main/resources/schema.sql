-- V1__init_schema.sql - SQLite Schema
-- ZJAdmin Database Initialization

-- 系统用户表
CREATE TABLE IF NOT EXISTS sys_user (
    id INTEGER PRIMARY KEY AUTOINCREMENT,
    username TEXT NOT NULL,
    password TEXT NOT NULL,
    nickname TEXT NOT NULL,
    email TEXT DEFAULT NULL,
    phone TEXT DEFAULT NULL,
    status INTEGER NOT NULL DEFAULT 1,
    is_deleted INTEGER NOT NULL DEFAULT 0,
    create_time TEXT NOT NULL,
    update_time TEXT DEFAULT NULL,
    login_fail_count INTEGER NOT NULL DEFAULT 0,
    lockout_end_time TEXT DEFAULT NULL
);
CREATE UNIQUE INDEX IF NOT EXISTS idx_user_username ON sys_user(username);

-- 系统角色表
CREATE TABLE IF NOT EXISTS sys_role (
    id INTEGER PRIMARY KEY AUTOINCREMENT,
    name TEXT NOT NULL,
    code TEXT NOT NULL,
    description TEXT DEFAULT NULL,
    create_time TEXT NOT NULL,
    update_time TEXT DEFAULT NULL
);
CREATE UNIQUE INDEX IF NOT EXISTS idx_role_code ON sys_role(code);

-- 系统权限表
CREATE TABLE IF NOT EXISTS sys_permission (
    id INTEGER PRIMARY KEY AUTOINCREMENT,
    parent_id INTEGER NOT NULL DEFAULT 0,
    name TEXT NOT NULL,
    code TEXT NOT NULL,
    type INTEGER NOT NULL,
    path TEXT DEFAULT NULL,
    component TEXT DEFAULT NULL,
    icon TEXT DEFAULT NULL,
    sort INTEGER NOT NULL DEFAULT 0,
    visible INTEGER NOT NULL DEFAULT 1,
    create_time TEXT NOT NULL,
    update_time TEXT DEFAULT NULL
);
CREATE UNIQUE INDEX IF NOT EXISTS idx_permission_code ON sys_permission(code);

-- 用户角色关联表
CREATE TABLE IF NOT EXISTS sys_user_role (
    id INTEGER PRIMARY KEY AUTOINCREMENT,
    user_id INTEGER NOT NULL,
    role_id INTEGER NOT NULL
);
CREATE UNIQUE INDEX IF NOT EXISTS idx_user_role ON sys_user_role(user_id, role_id);

-- 角色权限关联表
CREATE TABLE IF NOT EXISTS sys_role_permission (
    id INTEGER PRIMARY KEY AUTOINCREMENT,
    role_id INTEGER NOT NULL,
    permission_id INTEGER NOT NULL
);
CREATE UNIQUE INDEX IF NOT EXISTS idx_role_permission ON sys_role_permission(role_id, permission_id);

-- 系统配置表
CREATE TABLE IF NOT EXISTS sys_config (
    id INTEGER PRIMARY KEY AUTOINCREMENT,
    config_key TEXT NOT NULL,
    config_value TEXT DEFAULT NULL,
    description TEXT DEFAULT NULL,
    create_time TEXT NOT NULL,
    update_time TEXT DEFAULT NULL
);
CREATE UNIQUE INDEX IF NOT EXISTS idx_config_key ON sys_config(config_key);

-- 操作日志表
CREATE TABLE IF NOT EXISTS sys_operation_log (
    id INTEGER PRIMARY KEY AUTOINCREMENT,
    operator TEXT DEFAULT NULL,
    module TEXT DEFAULT NULL,
    operation_type TEXT DEFAULT NULL,
    request_url TEXT DEFAULT NULL,
    request_method TEXT DEFAULT NULL,
    request_params TEXT DEFAULT NULL,
    response_result TEXT DEFAULT NULL,
    ip_address TEXT DEFAULT NULL,
    status INTEGER NOT NULL DEFAULT 1,
    duration INTEGER NOT NULL DEFAULT 0,
    operation_time TEXT NOT NULL
);

-- 异常日志表
CREATE TABLE IF NOT EXISTS sys_exception_log (
    id INTEGER PRIMARY KEY AUTOINCREMENT,
    message TEXT DEFAULT NULL,
    exception_type TEXT DEFAULT NULL,
    stack_trace TEXT DEFAULT NULL,
    request_url TEXT DEFAULT NULL,
    request_method TEXT DEFAULT NULL,
    request_params TEXT DEFAULT NULL,
    ip_address TEXT DEFAULT NULL,
    operator TEXT DEFAULT NULL,
    occur_time TEXT NOT NULL
);
