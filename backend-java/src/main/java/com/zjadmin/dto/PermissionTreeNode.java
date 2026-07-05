package com.zjadmin.dto;

import lombok.Data;
import java.time.LocalDateTime;
import java.util.List;

@Data
public class PermissionTreeNode {
    private Long id;
    private Long parentId;
    private String name;
    private String code;
    private Integer type;
    private String path;
    private String component;
    private String icon;
    private Integer sort;
    private Integer visible;
    private LocalDateTime createTime;
    private List<PermissionTreeNode> children;
}
