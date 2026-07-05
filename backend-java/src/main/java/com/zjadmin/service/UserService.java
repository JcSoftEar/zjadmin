package com.zjadmin.service;

import com.zjadmin.dto.*;

public interface UserService {
    ApiResponse<?> getPaged(UserQueryRequest query);
    ApiResponse<?> getById(Long id);
    ApiResponse<?> create(CreateUserRequest request);
    ApiResponse<?> update(Long id, UpdateUserRequest request);
    ApiResponse<?> delete(Long id);
    ApiResponse<?> resetPassword(Long id);
    ApiResponse<?> setStatus(Long id, Integer status);
    ApiResponse<?> changePassword(Long userId, ChangePasswordRequest request);
    ApiResponse<?> updateProfile(Long userId, UpdateProfileRequest request);
}
