package com.zjadmin.service;

import com.zjadmin.dto.*;

public interface RoleService {
    ApiResponse<?> getPaged(RoleQueryRequest query);
    ApiResponse<?> getAll();
    ApiResponse<?> getById(Long id);
    ApiResponse<?> create(CreateRoleRequest request);
    ApiResponse<?> update(Long id, UpdateRoleRequest request);
    ApiResponse<?> delete(Long id);
    ApiResponse<?> assignPermissions(Long roleId, RolePermissionsRequest request);
    ApiResponse<?> getPermissionIds(Long roleId);
}
