package com.zjadmin.config;

import jakarta.servlet.*;
import jakarta.servlet.http.HttpServletRequest;
import jakarta.servlet.http.HttpServletResponse;
import org.springframework.core.Ordered;
import org.springframework.core.annotation.Order;
import org.springframework.stereotype.Component;
import org.springframework.web.util.ContentCachingRequestWrapper;
import org.springframework.web.util.ContentCachingResponseWrapper;

import java.io.IOException;

/**
 * 内容缓存过滤器
 * 将请求和响应包装为可重复读取的形式，供操作日志拦截器使用
 */
@Component
@Order(Ordered.HIGHEST_PRECEDENCE)
public class ContentCachingFilter implements Filter {

    @Override
    public void doFilter(ServletRequest request, ServletResponse response, FilterChain chain)
            throws IOException, ServletException {

        if (request instanceof HttpServletRequest && response instanceof HttpServletResponse) {
            ContentCachingRequestWrapper wrappedRequest = new ContentCachingRequestWrapper(
                    (HttpServletRequest) request);
            ContentCachingResponseWrapper wrappedResponse = new ContentCachingResponseWrapper(
                    (HttpServletResponse) response);

            chain.doFilter(wrappedRequest, wrappedResponse);

            // 确保响应体写回到客户端
            wrappedResponse.copyBodyToResponse();
        } else {
            chain.doFilter(request, response);
        }
    }
}
