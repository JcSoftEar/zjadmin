package com.zjadmin.controller;

import com.zjadmin.dto.*;
import com.zjadmin.security.Permission;
import com.zjadmin.service.UserService;
import jakarta.validation.Valid;
import lombok.RequiredArgsConstructor;
import org.springframework.web.bind.annotation.*;

@RestController
@RequestMapping("/api/v1/users")
@RequiredArgsConstructor
public class UserController {

    private final UserService userService;

    @GetMapping
    @Permission("system:user:query")
    public ApiResponse<?> getPaged(UserQueryRequest query) {
        return userService.getPaged(query);
    }

    @GetMapping("/{id}")
    @Permission("system:user:query")
    public ApiResponse<?> getById(@PathVariable Long id) {
        return userService.getById(id);
    }

    @PostMapping
    @Permission("system:user:add")
    public ApiResponse<?> create(@Valid @RequestBody CreateUserRequest request) {
        return userService.create(request);
    }

    @PutMapping("/{id}")
    @Permission("system:user:edit")
    public ApiResponse<?> update(@PathVariable Long id, @RequestBody UpdateUserRequest request) {
        return userService.update(id, request);
    }

    @DeleteMapping("/{id}")
    @Permission("system:user:delete")
    public ApiResponse<?> delete(@PathVariable Long id) {
        return userService.delete(id);
    }

    @PutMapping("/{id}/reset-password")
    @Permission("system:user:edit")
    public ApiResponse<?> resetPassword(@PathVariable Long id) {
        return userService.resetPassword(id);
    }

    @PutMapping("/{id}/status")
    @Permission("system:user:edit")
    public ApiResponse<?> setStatus(@PathVariable Long id, @RequestBody Integer status) {
        return userService.setStatus(id, status);
    }
}
