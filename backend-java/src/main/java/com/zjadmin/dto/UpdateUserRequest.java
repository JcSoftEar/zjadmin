package com.zjadmin.dto;

import lombok.Data;
import java.util.List;

@Data
public class UpdateUserRequest {
    private String nickname;
    private String email;
    private String phone;
    private List<Long> roleIds;
    private Integer status = 1;
}
