package com.zjadmin.service.impl;

import com.baomidou.mybatisplus.core.conditions.query.LambdaQueryWrapper;
import com.baomidou.mybatisplus.extension.plugins.pagination.Page;
import com.zjadmin.dto.*;
import com.zjadmin.entity.*;
import com.zjadmin.exception.BusinessException;
import com.zjadmin.mapper.*;
import com.zjadmin.service.UserService;
import lombok.RequiredArgsConstructor;
import org.springframework.security.crypto.password.PasswordEncoder;
import org.springframework.stereotype.Service;
import org.springframework.transaction.annotation.Transactional;
import org.springframework.util.StringUtils;

import java.time.LocalDateTime;
import java.util.*;
import java.util.stream.Collectors;

@Service
@RequiredArgsConstructor
public class UserServiceImpl implements UserService {

    private final UserMapper userMapper;
    private final UserRoleMapper userRoleMapper;
    private final RoleMapper roleMapper;
    private final PasswordEncoder passwordEncoder;

    @Override
    public ApiResponse<?> getPaged(UserQueryRequest query) {
        LambdaQueryWrapper<User> wrapper = new LambdaQueryWrapper<User>()
                .eq(User::getIsDeleted, 0);

        if (StringUtils.hasText(query.getUsername()))
            wrapper.like(User::getUsername, query.getUsername());
        if (StringUtils.hasText(query.getNickname()))
            wrapper.like(User::getNickname, query.getNickname());
        if (StringUtils.hasText(query.getEmail()))
            wrapper.like(User::getEmail, query.getEmail());
        if (StringUtils.hasText(query.getPhone()))
            wrapper.like(User::getPhone, query.getPhone());
        if (query.getStatus() != null)
            wrapper.eq(User::getStatus, query.getStatus());

        // 按角色过滤
        if (query.getRoleId() != null) {
            List<UserRole> userRoles = userRoleMapper.selectList(
                    new LambdaQueryWrapper<UserRole>().eq(UserRole::getRoleId, query.getRoleId()));
            Set<Long> userIds = userRoles.stream().map(UserRole::getUserId).collect(Collectors.toSet());
            if (userIds.isEmpty()) {
                return ApiResponse.successWithTotal(List.of(), 0);
            }
            wrapper.in(User::getId, userIds);
        }

        wrapper.orderByDesc(User::getId);

        Page<User> page = userMapper.selectPage(
                new Page<>(query.getPage(), query.getPageSize()), wrapper);

        List<Map<String, Object>> data = page.getRecords().stream()
                .map(this::buildUserWithRoles)
                .collect(Collectors.toList());

        return ApiResponse.successWithTotal(data, (int) page.getTotal());
    }

    @Override
    public ApiResponse<?> getById(Long id) {
        User user = userMapper.selectOne(
                new LambdaQueryWrapper<User>()
                        .eq(User::getId, id)
                        .eq(User::getIsDeleted, 0));

        if (user == null) return ApiResponse.error("用户不存在", 404);

        List<UserRole> userRoles = userRoleMapper.selectList(
                new LambdaQueryWrapper<UserRole>().eq(UserRole::getUserId, id));
        List<Long> roleIds = userRoles.stream().map(UserRole::getRoleId).collect(Collectors.toList());

        Map<String, Object> data = new LinkedHashMap<>();
        data.put("id", user.getId());
        data.put("username", user.getUsername());
        data.put("nickname", user.getNickname());
        data.put("email", user.getEmail());
        data.put("phone", user.getPhone());
        data.put("status", user.getStatus());
        data.put("roleIds", roleIds);
        data.put("createTime", user.getCreateTime());

        return ApiResponse.success(data);
    }

    @Override
    @Transactional
    public ApiResponse<?> create(CreateUserRequest request) {
        // 检查用户名是否存在
        Long count = userMapper.selectCount(
                new LambdaQueryWrapper<User>().eq(User::getUsername, request.getUsername()));
        if (count > 0) return ApiResponse.error("用户名已存在", 400);

        if (!request.getPassword().equals(request.getConfirmPassword()))
            return ApiResponse.error("两次密码不一致", 400);

        User user = new User();
        user.setUsername(request.getUsername());
        user.setPassword(passwordEncoder.encode(request.getPassword()));
        user.setNickname(request.getNickname());
        user.setEmail(request.getEmail());
        user.setPhone(request.getPhone());
        user.setStatus(request.getStatus());
        user.setCreateTime(LocalDateTime.now());
        userMapper.insert(user);

        // 分配角色
        if (request.getRoleIds() != null && !request.getRoleIds().isEmpty()) {
            for (Long roleId : request.getRoleIds()) {
                UserRole userRole = new UserRole();
                userRole.setUserId(user.getId());
                userRole.setRoleId(roleId);
                userRoleMapper.insert(userRole);
            }
        }

        return ApiResponse.success(Map.of("id", user.getId()), "创建成功");
    }

    @Override
    @Transactional
    public ApiResponse<?> update(Long id, UpdateUserRequest request) {
        User user = userMapper.selectOne(
                new LambdaQueryWrapper<User>()
                        .eq(User::getId, id)
                        .eq(User::getIsDeleted, 0));

        if (user == null) return ApiResponse.error("用户不存在", 404);

        user.setNickname(request.getNickname());
        user.setEmail(request.getEmail());
        user.setPhone(request.getPhone());
        user.setStatus(request.getStatus());
        user.setUpdateTime(LocalDateTime.now());
        userMapper.updateById(user);

        // 更新角色
        userRoleMapper.delete(
                new LambdaQueryWrapper<UserRole>().eq(UserRole::getUserId, id));

        if (request.getRoleIds() != null) {
            for (Long roleId : request.getRoleIds()) {
                UserRole userRole = new UserRole();
                userRole.setUserId(id);
                userRole.setRoleId(roleId);
                userRoleMapper.insert(userRole);
            }
        }

        return ApiResponse.success(null, "更新成功");
    }

    @Override
    public ApiResponse<?> delete(Long id) {
        User user = userMapper.selectById(id);
        if (user == null) return ApiResponse.error("用户不存在", 404);

        if ("admin".equals(user.getUsername()))
            return ApiResponse.error("不可删除超级管理员", 400);

        // 软删除
        user.setIsDeleted(1);
        user.setUpdateTime(LocalDateTime.now());
        userMapper.updateById(user);

        return ApiResponse.success(null, "删除成功");
    }

    @Override
    public ApiResponse<?> resetPassword(Long id) {
        User user = userMapper.selectById(id);
        if (user == null) return ApiResponse.error("用户不存在", 404);

        user.setPassword(passwordEncoder.encode("123456"));
        user.setUpdateTime(LocalDateTime.now());
        userMapper.updateById(user);

        return ApiResponse.success(null, "密码已重置为 123456");
    }

    @Override
    public ApiResponse<?> setStatus(Long id, Integer status) {
        User user = userMapper.selectById(id);
        if (user == null) return ApiResponse.error("用户不存在", 404);

        user.setStatus(status);
        user.setUpdateTime(LocalDateTime.now());
        userMapper.updateById(user);

        return ApiResponse.success(null, status == 1 ? "已启用" : "已禁用");
    }

    @Override
    public ApiResponse<?> changePassword(Long userId, ChangePasswordRequest request) {
        User user = userMapper.selectById(userId);
        if (user == null) return ApiResponse.error("用户不存在", 404);

        if (!passwordEncoder.matches(request.getOldPassword(), user.getPassword()))
            return ApiResponse.error("原密码错误", 400);

        if (!request.getNewPassword().equals(request.getConfirmPassword()))
            return ApiResponse.error("两次密码不一致", 400);

        user.setPassword(passwordEncoder.encode(request.getNewPassword()));
        user.setUpdateTime(LocalDateTime.now());
        userMapper.updateById(user);

        return ApiResponse.success(null, "密码修改成功");
    }

    @Override
    public ApiResponse<?> updateProfile(Long userId, UpdateProfileRequest request) {
        User user = userMapper.selectById(userId);
        if (user == null) return ApiResponse.error("用户不存在", 404);

        user.setNickname(request.getNickname());
        user.setEmail(request.getEmail());
        user.setPhone(request.getPhone());
        user.setUpdateTime(LocalDateTime.now());
        userMapper.updateById(user);

        return ApiResponse.success(null, "更新成功");
    }

    private Map<String, Object> buildUserWithRoles(User user) {
        List<UserRole> userRoles = userRoleMapper.selectList(
                new LambdaQueryWrapper<UserRole>().eq(UserRole::getUserId, user.getId()));

        List<Map<String, Object>> roles = userRoles.stream()
                .map(ur -> {
                    Role role = roleMapper.selectById(ur.getRoleId());
                    if (role == null) return null;
                    Map<String, Object> r = new LinkedHashMap<>();
                    r.put("id", role.getId());
                    r.put("name", role.getName());
                    r.put("code", role.getCode());
                    return r;
                })
                .filter(Objects::nonNull)
                .collect(Collectors.toList());

        Map<String, Object> data = new LinkedHashMap<>();
        data.put("id", user.getId());
        data.put("username", user.getUsername());
        data.put("nickname", user.getNickname());
        data.put("email", user.getEmail());
        data.put("phone", user.getPhone());
        data.put("status", user.getStatus());
        data.put("roles", roles);
        data.put("createTime", user.getCreateTime());
        data.put("updateTime", user.getUpdateTime());
        return data;
    }
}
