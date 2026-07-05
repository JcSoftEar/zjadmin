package com.zjadmin.interceptor;

import com.zjadmin.entity.OperationLog;
import com.zjadmin.mapper.OperationLogMapper;
import com.zjadmin.security.SecurityUtils;
import com.zjadmin.util.IpUtils;
import jakarta.servlet.http.HttpServletRequest;
import jakarta.servlet.http.HttpServletResponse;
import lombok.RequiredArgsConstructor;
import lombok.extern.slf4j.Slf4j;
import org.springframework.stereotype.Component;
import org.springframework.web.servlet.HandlerInterceptor;
import org.springframework.web.util.ContentCachingRequestWrapper;
import org.springframework.web.util.ContentCachingResponseWrapper;

import java.time.LocalDateTime;
import java.util.Map;

/**
 * 操作日志拦截器 - 替代原 .NET 的 OperationLogMiddleware
 */
@Slf4j
@Component
@RequiredArgsConstructor
public class OperationLogInterceptor implements HandlerInterceptor {

    private static final String START_TIME_ATTR = "operationLogStartTime";

    private final OperationLogMapper operationLogMapper;

    private static final Map<String, String> MODULE_MAP = Map.ofEntries(
            Map.entry("auth", "认证管理"),
            Map.entry("users", "用户管理"),
            Map.entry("roles", "角色管理"),
            Map.entry("permissions", "权限管理"),
            Map.entry("operation-logs", "操作日志"),
            Map.entry("exception-logs", "异常日志"),
            Map.entry("config", "系统设置")
    );

    private static final Map<String, String> METHOD_MAP = Map.of(
            "POST", "新增",
            "PUT", "修改",
            "DELETE", "删除",
            "GET", "查询"
    );

    @Override
    public boolean preHandle(HttpServletRequest request, HttpServletResponse response, Object handler) {
        request.setAttribute(START_TIME_ATTR, System.currentTimeMillis());
        return true;
    }

    @Override
    public void afterCompletion(HttpServletRequest request, HttpServletResponse response,
                                Object handler, Exception ex) {
        String path = request.getRequestURI();
        String method = request.getMethod();

        // 只记录 API 请求
        if (!path.startsWith("/api/")) return;

        // 跳过 GET 日志请求（太吵）
        if ("GET".equals(method) && path.contains("/logs/")) return;

        try {
            Long startTime = (Long) request.getAttribute(START_TIME_ATTR);
            long duration = startTime != null ? System.currentTimeMillis() - startTime : 0;

            // 解析模块和操作类型
            String module = parseModule(path);
            String operationType = METHOD_MAP.getOrDefault(method, "其他");

            // 获取操作人
            String username = SecurityUtils.getCurrentUsername();
            if (username == null) username = "匿名";

            // 读取请求体
            String requestBody = "";
            if (request instanceof ContentCachingRequestWrapper wrapper) {
                byte[] buf = wrapper.getContentAsByteArray();
                if (buf.length > 0) {
                    requestBody = new String(buf);
                }
            }
            if (requestBody.length() > 2000) {
                requestBody = requestBody.substring(0, 2000);
            }

            // 读取响应体
            String responseBody = "";
            if (response instanceof ContentCachingResponseWrapper wrapper) {
                byte[] buf = wrapper.getContentAsByteArray();
                if (buf.length > 0) {
                    responseBody = new String(buf);
                }
                wrapper.copyBodyToResponse();
            }
            if (responseBody.length() > 2000) {
                responseBody = responseBody.substring(0, 2000);
            }

            OperationLog opLog = new OperationLog();
            opLog.setOperator(username);
            opLog.setModule(module);
            opLog.setOperationType(operationType);
            opLog.setRequestUrl(path);
            opLog.setRequestMethod(method);
            opLog.setRequestParams(requestBody);
            opLog.setResponseResult(responseBody);
            opLog.setIpAddress(IpUtils.getClientIp(request));
            opLog.setStatus(response.getStatus() == 200 ? 1 : 0);
            opLog.setDuration(duration);
            opLog.setOperationTime(LocalDateTime.now());

            operationLogMapper.insert(opLog);
        } catch (Exception e) {
            log.warn("保存操作日志失败: {}", e.getMessage());
        }
    }

    private String parseModule(String path) {
        String[] parts = path.trim().split("/");
        if (parts.length < 4) return "其他";
        String resource = parts[3]; // /api/v1/{resource}/...
        return MODULE_MAP.getOrDefault(resource, "其他");
    }
}
