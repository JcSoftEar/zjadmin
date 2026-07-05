-- V1__init_schema.sql - MySQL Schema
-- ZJAdmin Database Initialization

-- 系统用户表
CREATE TABLE IF NOT EXISTS sys_user (
    id BIGINT AUTO_INCREMENT PRIMARY KEY,
    username VARCHAR(50) NOT NULL,
    password VARCHAR(100) NOT NULL,
    nickname VARCHAR(50) NOT NULL,
    email VARCHAR(100) DEFAULT NULL,
    phone VARCHAR(20) DEFAULT NULL,
    status TINYINT NOT NULL DEFAULT 1 COMMENT '0=禁用 1=启用',
    is_deleted TINYINT NOT NULL DEFAULT 0 COMMENT '0=正常 1=已删除',
    create_time DATETIME NOT NULL,
    update_time DATETIME DEFAULT NULL,
    login_fail_count INT NOT NULL DEFAULT 0,
    lockout_end_time DATETIME DEFAULT NULL,
    UNIQUE INDEX idx_user_username (username)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci COMMENT='系统用户表';

-- 系统角色表
CREATE TABLE IF NOT EXISTS sys_role (
    id BIGINT AUTO_INCREMENT PRIMARY KEY,
    name VARCHAR(50) NOT NULL,
    code VARCHAR(50) NOT NULL,
    description VARCHAR(200) DEFAULT NULL,
    create_time DATETIME NOT NULL,
    update_time DATETIME DEFAULT NULL,
    UNIQUE INDEX idx_role_code (code)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci COMMENT='系统角色表';

-- 系统权限表
CREATE TABLE IF NOT EXISTS sys_permission (
    id BIGINT AUTO_INCREMENT PRIMARY KEY,
    parent_id BIGINT NOT NULL DEFAULT 0 COMMENT '父级ID，0为顶级',
    name VARCHAR(50) NOT NULL,
    code VARCHAR(100) NOT NULL,
    type TINYINT NOT NULL COMMENT '0=菜单 1=按钮',
    path VARCHAR(200) DEFAULT NULL,
    component VARCHAR(200) DEFAULT NULL,
    icon VARCHAR(50) DEFAULT NULL,
    sort INT NOT NULL DEFAULT 0,
    visible TINYINT NOT NULL DEFAULT 1 COMMENT '0=隐藏 1=显示',
    create_time DATETIME NOT NULL,
    update_time DATETIME DEFAULT NULL,
    UNIQUE INDEX idx_permission_code (code)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci COMMENT='系统权限表';

-- 用户角色关联表
CREATE TABLE IF NOT EXISTS sys_user_role (
    id BIGINT AUTO_INCREMENT PRIMARY KEY,
    user_id BIGINT NOT NULL,
    role_id BIGINT NOT NULL,
    UNIQUE INDEX idx_user_role (user_id, role_id)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci COMMENT='用户角色关联表';

-- 角色权限关联表
CREATE TABLE IF NOT EXISTS sys_role_permission (
    id BIGINT AUTO_INCREMENT PRIMARY KEY,
    role_id BIGINT NOT NULL,
    permission_id BIGINT NOT NULL,
    UNIQUE INDEX idx_role_permission (role_id, permission_id)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci COMMENT='角色权限关联表';

-- 系统配置表
CREATE TABLE IF NOT EXISTS sys_config (
    id BIGINT AUTO_INCREMENT PRIMARY KEY,
    config_key VARCHAR(100) NOT NULL,
    config_value TEXT DEFAULT NULL,
    description VARCHAR(200) DEFAULT NULL,
    create_time DATETIME NOT NULL,
    update_time DATETIME DEFAULT NULL,
    UNIQUE INDEX idx_config_key (config_key)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci COMMENT='系统配置表';

-- 操作日志表
CREATE TABLE IF NOT EXISTS sys_operation_log (
    id BIGINT AUTO_INCREMENT PRIMARY KEY,
    operator VARCHAR(50) DEFAULT NULL,
    module VARCHAR(50) DEFAULT NULL,
    operation_type VARCHAR(20) DEFAULT NULL,
    request_url VARCHAR(500) DEFAULT NULL,
    request_method VARCHAR(10) DEFAULT NULL,
    request_params TEXT DEFAULT NULL,
    response_result TEXT DEFAULT NULL,
    ip_address VARCHAR(50) DEFAULT NULL,
    status TINYINT NOT NULL DEFAULT 1 COMMENT '0=失败 1=成功',
    duration BIGINT NOT NULL DEFAULT 0 COMMENT '耗时(ms)',
    operation_time DATETIME NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci COMMENT='操作日志表';

-- 异常日志表
CREATE TABLE IF NOT EXISTS sys_exception_log (
    id BIGINT AUTO_INCREMENT PRIMARY KEY,
    message TEXT DEFAULT NULL,
    exception_type VARCHAR(200) DEFAULT NULL,
    stack_trace TEXT DEFAULT NULL,
    request_url VARCHAR(500) DEFAULT NULL,
    request_method VARCHAR(10) DEFAULT NULL,
    request_params TEXT DEFAULT NULL,
    ip_address VARCHAR(50) DEFAULT NULL,
    operator VARCHAR(50) DEFAULT NULL,
    occur_time DATETIME NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci COMMENT='异常日志表';
