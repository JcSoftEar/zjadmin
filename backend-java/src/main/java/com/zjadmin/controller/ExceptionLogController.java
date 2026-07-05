package com.zjadmin.controller;

import com.zjadmin.dto.ApiResponse;
import com.zjadmin.dto.CleanLogRequest;
import com.zjadmin.dto.ExceptionLogQueryRequest;
import com.zjadmin.security.Permission;
import com.zjadmin.service.LogService;
import lombok.RequiredArgsConstructor;
import org.springframework.http.HttpHeaders;
import org.springframework.http.MediaType;
import org.springframework.http.ResponseEntity;
import org.springframework.web.bind.annotation.*;

@RestController
@RequestMapping("/api/v1/exception-logs")
@RequiredArgsConstructor
public class ExceptionLogController {

    private final LogService logService;

    @GetMapping
    @Permission("logs:exception:query")
    public ApiResponse<?> getPaged(ExceptionLogQueryRequest query) {
        return logService.getExceptionLogsPaged(query);
    }

    @GetMapping("/{id}")
    @Permission("logs:exception:query")
    public ApiResponse<?> getDetail(@PathVariable Long id) {
        return logService.getExceptionLogDetail(id);
    }

    @DeleteMapping("/clean")
    @Permission("logs:exception:delete")
    public ApiResponse<?> clean(@RequestBody CleanLogRequest request) {
        return logService.cleanExceptionLogs(request);
    }

    @GetMapping("/export")
    @Permission("logs:exception:query")
    public ResponseEntity<byte[]> export(ExceptionLogQueryRequest query) {
        byte[] data = logService.exportExceptionLogs(query);
        return ResponseEntity.ok()
                .header(HttpHeaders.CONTENT_DISPOSITION, "attachment; filename=exception-logs.xlsx")
                .contentType(MediaType.parseMediaType("application/vnd.openxmlformats-officedocument.spreadsheetml.sheet"))
                .body(data);
    }
}
