package com.zjadmin.controller;

import com.zjadmin.dto.*;
import com.zjadmin.security.Permission;
import com.zjadmin.service.RoleService;
import jakarta.validation.Valid;
import lombok.RequiredArgsConstructor;
import org.springframework.web.bind.annotation.*;

@RestController
@RequestMapping("/api/v1/roles")
@RequiredArgsConstructor
public class RoleController {

    private final RoleService roleService;

    @GetMapping
    @Permission("system:role:query")
    public ApiResponse<?> getPaged(RoleQueryRequest query) {
        return roleService.getPaged(query);
    }

    @GetMapping("/all")
    public ApiResponse<?> getAll() {
        return roleService.getAll();
    }

    @GetMapping("/{id}")
    @Permission("system:role:query")
    public ApiResponse<?> getById(@PathVariable Long id) {
        return roleService.getById(id);
    }

    @PostMapping
    @Permission("system:role:add")
    public ApiResponse<?> create(@Valid @RequestBody CreateRoleRequest request) {
        return roleService.create(request);
    }

    @PutMapping("/{id}")
    @Permission("system:role:edit")
    public ApiResponse<?> update(@PathVariable Long id, @RequestBody UpdateRoleRequest request) {
        return roleService.update(id, request);
    }

    @DeleteMapping("/{id}")
    @Permission("system:role:delete")
    public ApiResponse<?> delete(@PathVariable Long id) {
        return roleService.delete(id);
    }

    @PutMapping("/{id}/permissions")
    @Permission("system:role:edit")
    public ApiResponse<?> assignPermissions(@PathVariable Long id, @RequestBody RolePermissionsRequest request) {
        return roleService.assignPermissions(id, request);
    }

    @GetMapping("/{id}/permissions")
    @Permission("system:role:query")
    public ApiResponse<?> getPermissionIds(@PathVariable Long id) {
        return roleService.getPermissionIds(id);
    }
}
