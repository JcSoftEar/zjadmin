package com.zjadmin.service;

import com.zjadmin.dto.*;
import java.util.List;

public interface AuthService {
    ApiResponse<LoginResponse> login(LoginRequest request);
    UserInfo getCurrentUser(Long userId);
    List<String> getUserPermissions(Long userId);
}
