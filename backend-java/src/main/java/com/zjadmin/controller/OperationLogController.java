package com.zjadmin.controller;

import com.zjadmin.dto.ApiResponse;
import com.zjadmin.dto.CleanLogRequest;
import com.zjadmin.dto.OperationLogQueryRequest;
import com.zjadmin.security.Permission;
import com.zjadmin.service.LogService;
import lombok.RequiredArgsConstructor;
import org.springframework.http.HttpHeaders;
import org.springframework.http.MediaType;
import org.springframework.http.ResponseEntity;
import org.springframework.web.bind.annotation.*;

@RestController
@RequestMapping("/api/v1/operation-logs")
@RequiredArgsConstructor
public class OperationLogController {

    private final LogService logService;

    @GetMapping
    @Permission("logs:operation:query")
    public ApiResponse<?> getPaged(OperationLogQueryRequest query) {
        return logService.getOperationLogsPaged(query);
    }

    @GetMapping("/{id}")
    @Permission("logs:operation:query")
    public ApiResponse<?> getDetail(@PathVariable Long id) {
        return logService.getOperationLogDetail(id);
    }

    @DeleteMapping("/clean")
    @Permission("logs:operation:delete")
    public ApiResponse<?> clean(@RequestBody CleanLogRequest request) {
        return logService.cleanOperationLogs(request);
    }

    @GetMapping("/export")
    @Permission("logs:operation:query")
    public ResponseEntity<byte[]> export(OperationLogQueryRequest query) {
        byte[] data = logService.exportOperationLogs(query);
        return ResponseEntity.ok()
                .header(HttpHeaders.CONTENT_DISPOSITION, "attachment; filename=operation-logs.xlsx")
                .contentType(MediaType.parseMediaType("application/vnd.openxmlformats-officedocument.spreadsheetml.sheet"))
                .body(data);
    }
}
