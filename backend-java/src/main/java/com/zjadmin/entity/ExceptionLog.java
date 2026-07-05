package com.zjadmin.entity;

import com.baomidou.mybatisplus.annotation.*;
import lombok.Data;
import java.time.LocalDateTime;

@Data
@TableName("sys_exception_log")
public class ExceptionLog {

    @TableId(type = IdType.AUTO)
    private Long id;

    private String message;

    private String exceptionType;

    private String stackTrace;

    private String requestUrl;

    private String requestMethod;

    private String requestParams;

    private String ipAddress;

    private String operator;

    private LocalDateTime occurTime;
}
