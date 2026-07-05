package com.zjadmin.service;

import com.zjadmin.dto.ApiResponse;
import java.util.Map;

public interface ConfigService {
    ApiResponse<?> getAll();
    ApiResponse<?> update(Map<String, String> settings);
}
