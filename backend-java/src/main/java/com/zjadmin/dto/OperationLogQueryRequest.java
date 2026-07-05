package com.zjadmin.dto;

import lombok.Data;
import lombok.EqualsAndHashCode;
import java.time.LocalDateTime;

@Data
@EqualsAndHashCode(callSuper = true)
public class OperationLogQueryRequest extends PagedRequest {
    private String operator;
    private String module;
    private String operationType;
    private String ipAddress;
    private Integer status;
    private LocalDateTime startTime;
    private LocalDateTime endTime;
}
