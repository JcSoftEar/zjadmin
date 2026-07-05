package com.zjadmin.service.impl;

import com.baomidou.mybatisplus.core.conditions.query.LambdaQueryWrapper;
import com.zjadmin.dto.ApiResponse;
import com.zjadmin.entity.SysConfig;
import com.zjadmin.mapper.ConfigMapper;
import com.zjadmin.service.ConfigService;
import lombok.RequiredArgsConstructor;
import org.springframework.stereotype.Service;

import java.time.LocalDateTime;
import java.util.LinkedHashMap;
import java.util.List;
import java.util.Map;

@Service
@RequiredArgsConstructor
public class ConfigServiceImpl implements ConfigService {

    private final ConfigMapper configMapper;

    @Override
    public ApiResponse<?> getAll() {
        List<SysConfig> configs = configMapper.selectList(
                new LambdaQueryWrapper<SysConfig>().orderByAsc(SysConfig::getId));

        Map<String, String> data = new LinkedHashMap<>();
        for (SysConfig c : configs) {
            data.put(c.getConfigKey(), c.getConfigValue() != null ? c.getConfigValue() : "");
        }

        return ApiResponse.success(data);
    }

    @Override
    public ApiResponse<?> update(Map<String, String> settings) {
        for (Map.Entry<String, String> entry : settings.entrySet()) {
            String key = entry.getKey();
            String value = entry.getValue();

            SysConfig config = configMapper.selectOne(
                    new LambdaQueryWrapper<SysConfig>().eq(SysConfig::getConfigKey, key));

            if (config != null) {
                config.setConfigValue(value);
                config.setUpdateTime(LocalDateTime.now());
                configMapper.updateById(config);
            } else {
                SysConfig newConfig = new SysConfig();
                newConfig.setConfigKey(key);
                newConfig.setConfigValue(value);
                newConfig.setCreateTime(LocalDateTime.now());
                configMapper.insert(newConfig);
            }
        }

        return ApiResponse.success(null, "保存成功");
    }
}
