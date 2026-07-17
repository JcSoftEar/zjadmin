package com.zjadmin.service.impl;

import com.baomidou.mybatisplus.core.conditions.query.LambdaQueryWrapper;
import com.zjadmin.dto.*;
import com.zjadmin.entity.*;
import com.zjadmin.exception.BusinessException;
import com.zjadmin.mapper.*;
import com.zjadmin.security.JwtTokenProvider;
import com.zjadmin.service.AuthService;
import lombok.RequiredArgsConstructor;
import lombok.extern.slf4j.Slf4j;
import org.springframework.security.crypto.password.PasswordEncoder;
import org.springframework.stereotype.Service;
import org.springframework.util.StringUtils;

import java.time.LocalDateTime;
import java.util.*;
import java.util.concurrent.ConcurrentHashMap;
import java.util.stream.Collectors;

@Slf4j
@Service
@RequiredArgsConstructor
public class AuthServiceImpl implements AuthService {

    private final UserMapper userMapper;
    private final UserRoleMapper userRoleMapper;
    private final RoleMapper roleMapper;
    private final RolePermissionMapper rolePermissionMapper;
    private final PermissionMapper permissionMapper;
    private final ExceptionLogMapper exceptionLogMapper;
    private final JwtTokenProvider jwtTokenProvider;
    private final PasswordEncoder passwordEncoder;

    // 内存登录锁定（与原 .NET 版本一致）
    private static final ConcurrentHashMap<String, LoginAttempt> LOGIN_ATTEMPTS = new ConcurrentHashMap<>();

    @Override
    public ApiResponse<LoginResponse> login(LoginRequest request) {
        String lockoutKey = "login_" + request.getUsername();

        // 检查登录锁定
        LoginAttempt attempt = LOGIN_ATTEMPTS.get(lockoutKey);
        if (attempt != null && attempt.isLocked()) {
            long remaining = attempt.getLockoutEnd() != null
                    ? (attempt.getLockoutEnd().getNano() - LocalDateTime.now().getNano()) / 60_000_000_000L
                    : 0;
            return ApiResponse.error("账户已锁定，请 " + Math.max(remaining, 1) + " 分钟后重试", 400);
        }

        // 查找用户
        User user = userMapper.selectOne(
                new LambdaQueryWrapper<User>()
                        .eq(User::getUsername, request.getUsername())
                        .eq(User::getIsDeleted, 0));

        if (user == null || !passwordEncoder.matches(request.getPassword(), user.getPassword())) {
            // 记录失败
            recordFailedAttempt(lockoutKey);

            // 记录登录失败日志
            try {
                ExceptionLog exLog = new ExceptionLog();
                exLog.setMessage("登录失败：用户名或密码错误");
                exLog.setExceptionType("LoginFailed");
                exLog.setRequestUrl("/api/v1/auth/login");
                exLog.setRequestMethod("POST");
                exLog.setRequestParams("{\"username\":\"" + request.getUsername() + "\"}");
                exLog.setOperator(request.getUsername());
                exLog.setOccurTime(LocalDateTime.now());
                exceptionLogMapper.insert(exLog);
            } catch (Exception e) {
                log.warn("记录登录失败日志异常: {}", e.getMessage());
            }

            return ApiResponse.error("用户名或密码错误", 400);
        }

        if (user.getStatus() == 0) {
            return ApiResponse.error("账户已被禁用", 400);
        }

        // 清除登录失败记录
        LOGIN_ATTEMPTS.remove(lockoutKey);
        user.setLoginFailCount(0);
        user.setLockoutEndTime(null);
        userMapper.updateById(user);

        // 生成 JWT
        String token = jwtTokenProvider.generateToken(user.getId(), user.getUsername());

        // 获取用户权限和角色
        List<String> permissions = getUserPermissions(user.getId());
        List<String> roleCodes = getUserRoleCodes(user.getId());

        UserInfo userInfo = new UserInfo();
        userInfo.setId(user.getId());
        userInfo.setUsername(user.getUsername());
        userInfo.setNickname(user.getNickname());
        userInfo.setEmail(user.getEmail());
        userInfo.setPhone(user.getPhone());
        userInfo.setRoles(roleCodes);
        userInfo.setPermissions(permissions);

        LoginResponse loginResponse = new LoginResponse();
        loginResponse.setToken(token);
        loginResponse.setUser(userInfo);

        return ApiResponse.success(loginResponse, "登录成功");
    }

    @Override
    public UserInfo getCurrentUser(Long userId) {
        User user = userMapper.selectOne(
                new LambdaQueryWrapper<User>()
                        .eq(User::getId, userId)
                        .eq(User::getIsDeleted, 0));

        if (user == null || user.getStatus() == 0) return null;

        List<String> permissions = getUserPermissions(userId);
        List<String> roleCodes = getUserRoleCodes(userId);

        UserInfo userInfo = new UserInfo();
        userInfo.setId(user.getId());
        userInfo.setUsername(user.getUsername());
        userInfo.setNickname(user.getNickname());
        userInfo.setEmail(user.getEmail());
        userInfo.setPhone(user.getPhone());
        userInfo.setRoles(roleCodes);
        userInfo.setPermissions(permissions);

        return userInfo;
    }

    @Override
    public List<String> getUserPermissions(Long userId) {
        // 检查是否为超级管理员
        boolean isSuperAdmin = userRoleMapper.selectList(
                new LambdaQueryWrapper<UserRole>().eq(UserRole::getUserId, userId))
                .stream()
                .anyMatch(ur -> {
                    Role role = roleMapper.selectById(ur.getRoleId());
                    return role != null && "super_admin".equals(role.getCode());
                });

        if (isSuperAdmin) {
            return List.of("*:*:*");
        }

        // 获取用户角色对应的权限
        List<UserRole> userRoles = userRoleMapper.selectList(
                new LambdaQueryWrapper<UserRole>().eq(UserRole::getUserId, userId));

        Set<Long> roleIds = userRoles.stream().map(UserRole::getRoleId).collect(Collectors.toSet());
        if (roleIds.isEmpty()) return List.of();

        List<RolePermission> rolePermissions = rolePermissionMapper.selectList(
                new LambdaQueryWrapper<RolePermission>().in(RolePermission::getRoleId, roleIds));

        Set<Long> permissionIds = rolePermissions.stream()
                .map(RolePermission::getPermissionId).collect(Collectors.toSet());
        if (permissionIds.isEmpty()) return List.of();

        return permissionMapper.selectBatchIds(permissionIds).stream()
                .map(Permission::getCode)
                .distinct()
                .collect(Collectors.toList());
    }

    @Override
    public List<PermissionTreeNode> getCurrentUserMenus(Long userId) {
        // Get all menu-type, visible permissions
        List<Permission> allMenus = permissionMapper.selectList(
                new LambdaQueryWrapper<Permission>()
                        .eq(Permission::getType, 0)
                        .eq(Permission::getVisible, 1)
                        .orderByAsc(Permission::getSort));

        List<String> userPermissionCodes = getUserPermissions(userId);

        // Super admin sees all menus
        if (userPermissionCodes.contains("*:*:*")) {
            return buildMenuTree(allMenus, 0L);
        }

        Set<String> userCodes = new HashSet<>(userPermissionCodes);
        Map<Long, Permission> allDict = allMenus.stream()
                .collect(Collectors.toMap(Permission::getId, m -> m));
        Set<Long> accessibleIds = new HashSet<>();

        for (Permission menu : allMenus) {
            if (isMenuAccessible(menu, userCodes, userPermissionCodes)) {
                markPath(menu.getId(), allDict, accessibleIds);
            }
        }

        List<Permission> accessibleMenus = allMenus.stream()
                .filter(m -> accessibleIds.contains(m.getId()))
                .collect(Collectors.toList());

        return buildMenuTree(accessibleMenus, 0L);
    }

    private boolean isMenuAccessible(Permission menu, Set<String> userCodes, List<String> userPermissionCodes) {
        if (userCodes.contains(menu.getCode())) return true;
        return userPermissionCodes.stream().anyMatch(c -> c.startsWith(menu.getCode() + ":"));
    }

    private void markPath(Long id, Map<Long, Permission> allDict, Set<Long> accessibleIds) {
        while (id != 0L && accessibleIds.add(id)) {
            Permission parent = allDict.get(id);
            id = parent != null ? parent.getParentId() : 0L;
        }
    }

    private List<PermissionTreeNode> buildMenuTree(List<Permission> permissions, Long parentId) {
        return permissions.stream()
                .filter(p -> p.getParentId().equals(parentId))
                .sorted(Comparator.comparingInt(Permission::getSort))
                .map(p -> {
                    PermissionTreeNode node = new PermissionTreeNode();
                    node.setId(p.getId());
                    node.setParentId(p.getParentId());
                    node.setName(p.getName());
                    node.setCode(p.getCode());
                    node.setType(p.getType());
                    node.setPath(p.getPath());
                    node.setComponent(p.getComponent());
                    node.setIcon(p.getIcon());
                    node.setSort(p.getSort());
                    node.setVisible(p.getVisible());
                    node.setCreateTime(p.getCreateTime());
                    node.setChildren(buildMenuTree(permissions, p.getId()));
                    return node;
                })
                .collect(Collectors.toList());
    }

    private List<String> getUserRoleCodes(Long userId) {
        List<UserRole> userRoles = userRoleMapper.selectList(
                new LambdaQueryWrapper<UserRole>().eq(UserRole::getUserId, userId));

        return userRoles.stream()
                .map(ur -> roleMapper.selectById(ur.getRoleId()))
                .filter(Objects::nonNull)
                .map(Role::getCode)
                .collect(Collectors.toList());
    }

    private void recordFailedAttempt(String lockoutKey) {
        LOGIN_ATTEMPTS.compute(lockoutKey, (key, existing) -> {
            if (existing == null) {
                LoginAttempt a = new LoginAttempt();
                a.setCount(1);
                a.setLastAttempt(LocalDateTime.now());
                return a;
            } else {
                existing.setCount(existing.getCount() + 1);
                existing.setLastAttempt(LocalDateTime.now());
                if (existing.getCount() >= 5) {
                    existing.setLockoutEnd(LocalDateTime.now().plusMinutes(15));
                }
                return existing;
            }
        });
    }

    private static class LoginAttempt {
        private int count;
        private LocalDateTime lastAttempt;
        private LocalDateTime lockoutEnd;

        public int getCount() { return count; }
        public void setCount(int count) { this.count = count; }
        public LocalDateTime getLastAttempt() { return lastAttempt; }
        public void setLastAttempt(LocalDateTime lastAttempt) { this.lastAttempt = lastAttempt; }
        public LocalDateTime getLockoutEnd() { return lockoutEnd; }
        public void setLockoutEnd(LocalDateTime lockoutEnd) { this.lockoutEnd = lockoutEnd; }

        public boolean isLocked() {
            return lockoutEnd != null && lockoutEnd.isAfter(LocalDateTime.now());
        }
    }
}
