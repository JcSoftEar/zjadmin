package com.zjadmin.service.impl;

import com.baomidou.mybatisplus.core.conditions.query.LambdaQueryWrapper;
import com.zjadmin.dto.*;
import com.zjadmin.entity.Permission;
import com.zjadmin.entity.RolePermission;
import com.zjadmin.mapper.PermissionMapper;
import com.zjadmin.mapper.RolePermissionMapper;
import com.zjadmin.service.PermissionService;
import lombok.RequiredArgsConstructor;
import org.springframework.stereotype.Service;

import java.time.LocalDateTime;
import java.util.ArrayList;
import java.util.List;
import java.util.stream.Collectors;

@Service
@RequiredArgsConstructor
public class PermissionServiceImpl implements PermissionService {

    private final PermissionMapper permissionMapper;
    private final RolePermissionMapper rolePermissionMapper;

    @Override
    public ApiResponse<?> getTree() {
        List<Permission> permissions = permissionMapper.selectList(
                new LambdaQueryWrapper<Permission>().orderByAsc(Permission::getSort));
        List<PermissionTreeNode> tree = buildTree(permissions, 0L);
        return ApiResponse.success(tree);
    }

    @Override
    public ApiResponse<?> getById(Long id) {
        Permission permission = permissionMapper.selectById(id);
        if (permission == null) return ApiResponse.error("权限不存在", 404);
        return ApiResponse.success(permission);
    }

    @Override
    public ApiResponse<?> create(CreatePermissionRequest request) {
        Long count = permissionMapper.selectCount(
                new LambdaQueryWrapper<Permission>().eq(Permission::getCode, request.getCode()));
        if (count > 0) return ApiResponse.error("权限标识已存在", 400);

        Permission permission = new Permission();
        permission.setParentId(request.getParentId());
        permission.setName(request.getName());
        permission.setCode(request.getCode());
        permission.setType(request.getType());
        permission.setPath(request.getPath());
        permission.setComponent(request.getComponent());
        permission.setIcon(request.getIcon());
        permission.setSort(request.getSort());
        permission.setVisible(request.getVisible());
        permission.setCreateTime(LocalDateTime.now());
        permissionMapper.insert(permission);

        return ApiResponse.success(java.util.Map.of("id", permission.getId()), "创建成功");
    }

    @Override
    public ApiResponse<?> update(Long id, UpdatePermissionRequest request) {
        Permission permission = permissionMapper.selectById(id);
        if (permission == null) return ApiResponse.error("权限不存在", 404);

        permission.setParentId(request.getParentId());
        permission.setName(request.getName());
        permission.setType(request.getType());
        permission.setPath(request.getPath());
        permission.setComponent(request.getComponent());
        permission.setIcon(request.getIcon());
        permission.setSort(request.getSort());
        permission.setVisible(request.getVisible());
        permission.setUpdateTime(LocalDateTime.now());
        permissionMapper.updateById(permission);

        return ApiResponse.success(null, "更新成功");
    }

    @Override
    public ApiResponse<?> delete(Long id) {
        Permission permission = permissionMapper.selectById(id);
        if (permission == null) return ApiResponse.error("权限不存在", 404);

        // 检查是否有子权限
        Long childCount = permissionMapper.selectCount(
                new LambdaQueryWrapper<Permission>().eq(Permission::getParentId, id));
        if (childCount > 0)
            return ApiResponse.error("该权限下有子权限，无法删除", 400);

        // 检查是否已分配给角色
        Long roleCount = rolePermissionMapper.selectCount(
                new LambdaQueryWrapper<RolePermission>().eq(RolePermission::getPermissionId, id));
        if (roleCount > 0)
            return ApiResponse.error("该权限已分配给 " + roleCount + " 个角色，无法删除", 400);

        permissionMapper.deleteById(id);
        return ApiResponse.success(null, "删除成功");
    }

    private List<PermissionTreeNode> buildTree(List<Permission> permissions, Long parentId) {
        return permissions.stream()
                .filter(p -> p.getParentId().equals(parentId))
                .sorted((a, b) -> a.getSort() - b.getSort())
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
                    node.setChildren(buildTree(permissions, p.getId()));
                    return node;
                })
                .collect(Collectors.toList());
    }
}
