package com.zjadmin.dto;

import lombok.Data;
import java.util.Map;

@Data
public class ConfigUpdateRequest {
    private Map<String, String> settings;
}
