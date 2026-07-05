package com.zjadmin.security;

import lombok.AllArgsConstructor;
import lombok.Data;

/**
 * JWT 认证后存储在 SecurityContext 中的用户信息
 */
@Data
@AllArgsConstructor
public class JwtUserPrincipal {
    private Long userId;
    private String username;
}
