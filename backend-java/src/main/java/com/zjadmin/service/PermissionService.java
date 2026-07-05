package com.zjadmin.service;

import com.zjadmin.dto.*;

public interface PermissionService {
    ApiResponse<?> getTree();
    ApiResponse<?> getById(Long id);
    ApiResponse<?> create(CreatePermissionRequest request);
    ApiResponse<?> update(Long id, UpdatePermissionRequest request);
    ApiResponse<?> delete(Long id);
}
