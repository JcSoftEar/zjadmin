package com.zjadmin.security;

import com.zjadmin.exception.ForbiddenException;
import com.zjadmin.exception.UnauthorizedException;
import com.zjadmin.service.AuthService;
import lombok.RequiredArgsConstructor;
import org.aspectj.lang.ProceedingJoinPoint;
import org.aspectj.lang.annotation.Around;
import org.aspectj.lang.annotation.Aspect;
import org.aspectj.lang.reflect.MethodSignature;
import org.springframework.stereotype.Component;

import java.lang.reflect.Method;
import java.util.List;

/**
 * 权限检查切面 - 替代原 .NET 的 PermissionAuthorizationFilter
 */
@Aspect
@Component
@RequiredArgsConstructor
public class PermissionAspect {

    private final AuthService authService;

    @Around("@annotation(com.zjadmin.security.Permission)")
    public Object checkPermission(ProceedingJoinPoint joinPoint) throws Throwable {
        MethodSignature signature = (MethodSignature) joinPoint.getSignature();
        Method method = signature.getMethod();

        Permission permission = method.getAnnotation(Permission.class);
        if (permission == null) {
            return joinPoint.proceed();
        }

        Long userId = SecurityUtils.getCurrentUserId();
        if (userId == null) {
            throw new UnauthorizedException("未认证");
        }

        List<String> userPermissions = authService.getUserPermissions(userId);

        // super_admin 通配权限
        if (userPermissions.contains("*:*:*")) {
            return joinPoint.proceed();
        }

        if (!userPermissions.contains(permission.value())) {
            throw new ForbiddenException("无权限访问");
        }

        return joinPoint.proceed();
    }
}
