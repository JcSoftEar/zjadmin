package com.zjadmin.controller;

import com.zjadmin.dto.ApiResponse;
import com.zjadmin.security.Permission;
import com.zjadmin.service.ConfigService;
import lombok.RequiredArgsConstructor;
import org.springframework.web.bind.annotation.*;

import java.util.Map;

@RestController
@RequestMapping("/api/v1/config")
@RequiredArgsConstructor
public class ConfigController {

    private final ConfigService configService;

    @GetMapping
    public ApiResponse<?> getAll() {
        return configService.getAll();
    }

    @PutMapping
    @Permission("system:config:edit")
    public ApiResponse<?> update(@RequestBody Map<String, String> settings) {
        return configService.update(settings);
    }
}
