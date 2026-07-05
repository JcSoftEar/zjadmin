package com.zjadmin.dto;

import lombok.Data;
import java.util.List;

@Data
public class LoginResponse {
    private String token;
    private UserInfo user;
}
