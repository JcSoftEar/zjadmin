package com.zjadmin.service.impl;

import com.baomidou.mybatisplus.core.conditions.query.LambdaQueryWrapper;
import com.baomidou.mybatisplus.extension.plugins.pagination.Page;
import com.zjadmin.dto.*;
import com.zjadmin.entity.*;
import com.zjadmin.mapper.*;
import com.zjadmin.service.RoleService;
import lombok.RequiredArgsConstructor;
import org.springframework.stereotype.Service;
import org.springframework.transaction.annotation.Transactional;
import org.springframework.util.StringUtils;

import java.time.LocalDateTime;
import java.util.List;
import java.util.stream.Collectors;

@Service
@RequiredArgsConstructor
public class RoleServiceImpl implements RoleService {

    private final RoleMapper roleMapper;
    private final UserRoleMapper userRoleMapper;
    private final RolePermissionMapper rolePermissionMapper;

    @Override
    public ApiResponse<?> getPaged(RoleQueryRequest query) {
        LambdaQueryWrapper<Role> wrapper = new LambdaQueryWrapper<>();

        if (StringUtils.hasText(query.getName()))
            wrapper.like(Role::getName, query.getName());
        if (StringUtils.hasText(query.getCode()))
            wrapper.like(Role::getCode, query.getCode());

        wrapper.orderByDesc(Role::getId);

        Page<Role> page = roleMapper.selectPage(
                new Page<>(query.getPage(), query.getPageSize()), wrapper);

        return ApiResponse.successWithTotal(page.getRecords(), (int) page.getTotal());
    }

    @Override
    public ApiResponse<?> getAll() {
        List<Role> roles = roleMapper.selectList(
                new LambdaQueryWrapper<Role>().orderByAsc(Role::getId));
        return ApiResponse.success(roles);
    }

    @Override
    public ApiResponse<?> getById(Long id) {
        Role role = roleMapper.selectById(id);
        if (role == null) return ApiResponse.error("角色不存在", 404);
        return ApiResponse.success(role);
    }

    @Override
    public ApiResponse<?> create(CreateRoleRequest request) {
        Long count = roleMapper.selectCount(
                new LambdaQueryWrapper<Role>().eq(Role::getCode, request.getCode()));
        if (count > 0) return ApiResponse.error("角色标识已存在", 400);

        Role role = new Role();
        role.setName(request.getName());
        role.setCode(request.getCode());
        role.setDescription(request.getDescription());
        role.setCreateTime(LocalDateTime.now());
        roleMapper.insert(role);

        return ApiResponse.success(java.util.Map.of("id", role.getId()), "创建成功");
    }

    @Override
    public ApiResponse<?> update(Long id, UpdateRoleRequest request) {
        Role role = roleMapper.selectById(id);
        if (role == null) return ApiResponse.error("角色不存在", 404);

        if (request.getName() != null) role.setName(request.getName());
        if (request.getDescription() != null) role.setDescription(request.getDescription());
        role.setUpdateTime(LocalDateTime.now());
        roleMapper.updateById(role);

        return ApiResponse.success(null, "更新成功");
    }

    @Override
    public ApiResponse<?> delete(Long id) {
        Role role = roleMapper.selectById(id);
        if (role == null) return ApiResponse.error("角色不存在", 404);

        if ("super_admin".equals(role.getCode()))
            return ApiResponse.error("不可删除超级管理员角色", 400);

        Long userCount = userRoleMapper.selectCount(
                new LambdaQueryWrapper<UserRole>().eq(UserRole::getRoleId, id));
        if (userCount > 0)
            return ApiResponse.error("该角色下还有 " + userCount + " 个用户，无法删除", 400);

        roleMapper.deleteById(id);
        return ApiResponse.success(null, "删除成功");
    }

    @Override
    @Transactional
    public ApiResponse<?> assignPermissions(Long roleId, RolePermissionsRequest request) {
        Role role = roleMapper.selectById(roleId);
        if (role == null) return ApiResponse.error("角色不存在", 404);

        // 先删后增
        rolePermissionMapper.delete(
                new LambdaQueryWrapper<RolePermission>().eq(RolePermission::getRoleId, roleId));

        if (request.getPermissionIds() != null) {
            for (Long permissionId : request.getPermissionIds()) {
                RolePermission rp = new RolePermission();
                rp.setRoleId(roleId);
                rp.setPermissionId(permissionId);
                rolePermissionMapper.insert(rp);
            }
        }

        return ApiResponse.success(null, "权限分配成功");
    }

    @Override
    public ApiResponse<?> getPermissionIds(Long roleId) {
        Long count = roleMapper.selectCount(
                new LambdaQueryWrapper<Role>().eq(Role::getId, roleId));
        if (count == 0) return ApiResponse.error("角色不存在", 404);

        List<Long> permissionIds = rolePermissionMapper.selectList(
                new LambdaQueryWrapper<RolePermission>().eq(RolePermission::getRoleId, roleId))
                .stream()
                .map(RolePermission::getPermissionId)
                .collect(Collectors.toList());

        return ApiResponse.success(permissionIds);
    }
}
