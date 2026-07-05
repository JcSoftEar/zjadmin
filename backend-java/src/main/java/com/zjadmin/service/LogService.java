package com.zjadmin.service;

import com.zjadmin.dto.*;

public interface LogService {
    ApiResponse<?> getOperationLogsPaged(OperationLogQueryRequest query);
    ApiResponse<?> getOperationLogDetail(Long id);
    ApiResponse<?> cleanOperationLogs(CleanLogRequest request);
    byte[] exportOperationLogs(OperationLogQueryRequest query);

    ApiResponse<?> getExceptionLogsPaged(ExceptionLogQueryRequest query);
    ApiResponse<?> getExceptionLogDetail(Long id);
    ApiResponse<?> cleanExceptionLogs(CleanLogRequest request);
    byte[] exportExceptionLogs(ExceptionLogQueryRequest query);
}
