package com.zjadmin.dto;

import lombok.Data;
import lombok.EqualsAndHashCode;

@Data
@EqualsAndHashCode(callSuper = true)
public class UserQueryRequest extends PagedRequest {
    private String username;
    private String nickname;
    private String email;
    private String phone;
    private Integer status;
    private Long roleId;
}
