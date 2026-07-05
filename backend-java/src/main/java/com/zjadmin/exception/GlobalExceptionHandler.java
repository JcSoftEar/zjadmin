package com.zjadmin.exception;

import com.zjadmin.dto.ApiResponse;
import com.zjadmin.entity.ExceptionLog;
import com.zjadmin.mapper.ExceptionLogMapper;
import com.zjadmin.security.SecurityUtils;
import com.zjadmin.util.IpUtils;
import jakarta.servlet.http.HttpServletRequest;
import lombok.RequiredArgsConstructor;
import lombok.extern.slf4j.Slf4j;
import org.springframework.http.HttpStatus;
import org.springframework.web.bind.annotation.ExceptionHandler;
import org.springframework.web.bind.annotation.ResponseStatus;
import org.springframework.web.bind.annotation.RestControllerAdvice;

import java.time.LocalDateTime;

/**
 * 全局异常处理 - 替代原 .NET 的 ExceptionMiddleware
 */
@Slf4j
@RestControllerAdvice
@RequiredArgsConstructor
public class GlobalExceptionHandler {

    private final ExceptionLogMapper exceptionLogMapper;

    @ExceptionHandler(BusinessException.class)
    @ResponseStatus(HttpStatus.OK)
    public ApiResponse<?> handleBusiness(BusinessException e) {
        return ApiResponse.error(e.getMessage(), e.getCode());
    }

    @ExceptionHandler(UnauthorizedException.class)
    @ResponseStatus(HttpStatus.UNAUTHORIZED)
    public ApiResponse<?> handleUnauthorized(UnauthorizedException e) {
        return ApiResponse.unauthorized(e.getMessage());
    }

    @ExceptionHandler(ForbiddenException.class)
    @ResponseStatus(HttpStatus.FORBIDDEN)
    public ApiResponse<?> handleForbidden(ForbiddenException e) {
        return ApiResponse.forbidden(e.getMessage());
    }

    @ExceptionHandler(Exception.class)
    @ResponseStatus(HttpStatus.INTERNAL_SERVER_ERROR)
    public ApiResponse<?> handleException(Exception e, HttpServletRequest request) {
        log.error("未处理的异常: {}", e.getMessage(), e);

        // 写入异常日志到数据库
        try {
            ExceptionLog exLog = new ExceptionLog();
            exLog.setMessage(e.getMessage());
            exLog.setExceptionType(e.getClass().getName());
            exLog.setStackTrace(getStackTraceStr(e));
            exLog.setRequestUrl(request.getRequestURI());
            exLog.setRequestMethod(request.getMethod());
            exLog.setIpAddress(IpUtils.getClientIp(request));
            exLog.setOperator(SecurityUtils.getCurrentUsername());
            exLog.setOccurTime(LocalDateTime.now());

            // 读取请求参数（截断2000字符）
            String params = "";
            if ("POST".equalsIgnoreCase(request.getMethod()) || "PUT".equalsIgnoreCase(request.getMethod())) {
                // request body 已在拦截器中处理，这里只记录 query string
                params = request.getQueryString() != null ? request.getQueryString() : "";
            }
            if (params.length() > 2000) {
                params = params.substring(0, 2000);
            }
            exLog.setRequestParams(params);

            exceptionLogMapper.insert(exLog);
        } catch (Exception dbEx) {
            log.warn("保存异常日志失败: {}", dbEx.getMessage());
        }

        return ApiResponse.error("服务器内部错误");
    }

    private String getStackTraceStr(Exception e) {
        StringBuilder sb = new StringBuilder();
        for (StackTraceElement element : e.getStackTrace()) {
            sb.append(element.toString()).append("\n");
            if (sb.length() > 4000) break;
        }
        return sb.toString();
    }
}
