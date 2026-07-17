package com.zjadmin.controller;

import com.zjadmin.dto.*;
import com.zjadmin.security.SecurityUtils;
import com.zjadmin.service.AuthService;
import com.zjadmin.service.UserService;
import jakarta.validation.Valid;
import lombok.RequiredArgsConstructor;
import org.springframework.security.crypto.password.PasswordEncoder;
import org.springframework.web.bind.annotation.*;

import java.util.List;

@RestController
@RequestMapping("/api/v1/auth")
@RequiredArgsConstructor
public class AuthController {

    private final AuthService authService;
    private final UserService userService;
    private final PasswordEncoder passwordEncoder;

    @PostMapping("/login")
    public ApiResponse<LoginResponse> login(@Valid @RequestBody LoginRequest request) {
        return authService.login(request);
    }

    @PostMapping("/logout")
    public ApiResponse<?> logout() {
        return ApiResponse.success(null, "Logout success");
    }

    @GetMapping("/menus")
    public ApiResponse<List<PermissionTreeNode>> getMenus() {
        Long userId = SecurityUtils.getCurrentUserId();
        List<PermissionTreeNode> menus = authService.getCurrentUserMenus(userId);
        return ApiResponse.success(menus);
    }

    @GetMapping("/profile")
    public ApiResponse<UserInfo> getProfile() {
        Long userId = SecurityUtils.getCurrentUserId();
        UserInfo user = authService.getCurrentUser(userId);
        if (user == null) return ApiResponse.error("User not found", 404);
        return ApiResponse.success(user);
    }

    @PutMapping("/profile")
    public ApiResponse<?> updateProfile(@RequestBody UpdateProfileRequest request) {
        Long userId = SecurityUtils.getCurrentUserId();
        return userService.updateProfile(userId, request);
    }

    @PutMapping("/password")
    public ApiResponse<?> changePassword(@Valid @RequestBody ChangePasswordRequest request) {
        Long userId = SecurityUtils.getCurrentUserId();
        return userService.changePassword(userId, request);
    }

    @GetMapping("/hash")
    public ApiResponse<String> generateHash(@RequestParam String password) {
        return ApiResponse.success(passwordEncoder.encode(password));
    }
}