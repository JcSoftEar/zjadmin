package com.zjadmin.entity;

import com.baomidou.mybatisplus.annotation.*;
import lombok.Data;
import java.time.LocalDateTime;

@Data
@TableName("sys_operation_log")
public class OperationLog {

    @TableId(type = IdType.AUTO)
    private Long id;

    private String operator;

    private String module;

    private String operationType;

    private String requestUrl;

    private String requestMethod;

    private String requestParams;

    private String responseResult;

    private String ipAddress;

    private Integer status;

    private Long duration;

    private LocalDateTime operationTime;
}
