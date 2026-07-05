package com.zjadmin.controller;

import com.zjadmin.dto.*;
import com.zjadmin.security.Permission;
import com.zjadmin.service.PermissionService;
import jakarta.validation.Valid;
import lombok.RequiredArgsConstructor;
import org.springframework.web.bind.annotation.*;

@RestController
@RequestMapping("/api/v1/permissions")
@RequiredArgsConstructor
public class PermissionController {

    private final PermissionService permissionService;

    @GetMapping("/tree")
    @Permission("system:permission:query")
    public ApiResponse<?> getTree() {
        return permissionService.getTree();
    }

    @GetMapping("/{id}")
    @Permission("system:permission:query")
    public ApiResponse<?> getById(@PathVariable Long id) {
        return permissionService.getById(id);
    }

    @PostMapping
    @Permission("system:permission:add")
    public ApiResponse<?> create(@Valid @RequestBody CreatePermissionRequest request) {
        return permissionService.create(request);
    }

    @PutMapping("/{id}")
    @Permission("system:permission:edit")
    public ApiResponse<?> update(@PathVariable Long id, @RequestBody UpdatePermissionRequest request) {
        return permissionService.update(id, request);
    }

    @DeleteMapping("/{id}")
    @Permission("system:permission:delete")
    public ApiResponse<?> delete(@PathVariable Long id) {
        return permissionService.delete(id);
    }
}
