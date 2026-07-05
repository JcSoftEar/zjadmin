package com.zjadmin.dto;

import lombok.Data;
import lombok.EqualsAndHashCode;

@Data
@EqualsAndHashCode(callSuper = true)
public class RoleQueryRequest extends PagedRequest {
    private String name;
    private String code;
}
