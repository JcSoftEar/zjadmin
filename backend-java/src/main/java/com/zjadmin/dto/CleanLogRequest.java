package com.zjadmin.dto;

import lombok.Data;
import java.time.LocalDateTime;

@Data
public class CleanLogRequest {
    private LocalDateTime startTime;
    private LocalDateTime endTime;
}
