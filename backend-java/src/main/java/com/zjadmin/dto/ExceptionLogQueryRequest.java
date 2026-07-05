package com.zjadmin.dto;

import lombok.Data;
import lombok.EqualsAndHashCode;
import java.time.LocalDateTime;

@Data
@EqualsAndHashCode(callSuper = true)
public class ExceptionLogQueryRequest extends PagedRequest {
    private String message;
    private String exceptionType;
    private String operator;
    private String ipAddress;
    private LocalDateTime startTime;
    private LocalDateTime endTime;
}
