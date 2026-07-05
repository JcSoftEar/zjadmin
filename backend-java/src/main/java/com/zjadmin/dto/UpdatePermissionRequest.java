package com.zjadmin.dto;

import jakarta.validation.constraints.NotBlank;
import lombok.Data;

@Data
public class UpdatePermissionRequest {
    private Long parentId = 0L;

    @NotBlank(message = "权限名称不能为空")
    private String name;

    private Integer type;
    private String path;
    private String component;
    private String icon;
    private Integer sort = 0;
    private Integer visible = 1;
}
