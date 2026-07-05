package com.zjadmin.service.impl;

import com.alibaba.excel.EasyExcel;
import com.baomidou.mybatisplus.core.conditions.query.LambdaQueryWrapper;
import com.baomidou.mybatisplus.extension.plugins.pagination.Page;
import com.zjadmin.dto.*;
import com.zjadmin.entity.ExceptionLog;
import com.zjadmin.entity.OperationLog;
import com.zjadmin.mapper.ExceptionLogMapper;
import com.zjadmin.mapper.OperationLogMapper;
import com.zjadmin.service.LogService;
import lombok.RequiredArgsConstructor;
import org.springframework.stereotype.Service;
import org.springframework.util.StringUtils;

import java.io.ByteArrayOutputStream;
import java.time.format.DateTimeFormatter;
import java.util.*;
import java.util.stream.Collectors;

@Service
@RequiredArgsConstructor
public class LogServiceImpl implements LogService {

    private final OperationLogMapper operationLogMapper;
    private final ExceptionLogMapper exceptionLogMapper;

    private static final DateTimeFormatter DTF = DateTimeFormatter.ofPattern("yyyy-MM-dd HH:mm:ss");

    // ========== 操作日志 ==========

    @Override
    public ApiResponse<?> getOperationLogsPaged(OperationLogQueryRequest query) {
        LambdaQueryWrapper<OperationLog> wrapper = buildOpLogWrapper(query);
        wrapper.orderByDesc(OperationLog::getId);

        Page<OperationLog> page = operationLogMapper.selectPage(
                new Page<>(query.getPage(), query.getPageSize()), wrapper);

        return ApiResponse.successWithTotal(page.getRecords(), (int) page.getTotal());
    }

    @Override
    public ApiResponse<?> getOperationLogDetail(Long id) {
        OperationLog log = operationLogMapper.selectById(id);
        if (log == null) return ApiResponse.error("日志不存在", 404);
        return ApiResponse.success(log);
    }

    @Override
    public ApiResponse<?> cleanOperationLogs(CleanLogRequest request) {
        LambdaQueryWrapper<OperationLog> wrapper = new LambdaQueryWrapper<>();
        if (request.getStartTime() != null)
            wrapper.ge(OperationLog::getOperationTime, request.getStartTime());
        if (request.getEndTime() != null)
            wrapper.le(OperationLog::getOperationTime, request.getEndTime());

        operationLogMapper.delete(wrapper);
        return ApiResponse.success(null, "清理成功");
    }

    @Override
    public byte[] exportOperationLogs(OperationLogQueryRequest query) {
        LambdaQueryWrapper<OperationLog> wrapper = buildOpLogWrapper(query);
        wrapper.orderByDesc(OperationLog::getId);
        List<OperationLog> logs = operationLogMapper.selectList(wrapper);

        List<Map<String, Object>> exportData = logs.stream().map(l -> {
            Map<String, Object> row = new LinkedHashMap<>();
            row.put("ID", l.getId());
            row.put("操作人", l.getOperator());
            row.put("模块", l.getModule());
            row.put("操作类型", l.getOperationType());
            row.put("请求地址", l.getRequestUrl());
            row.put("请求方法", l.getRequestMethod());
            row.put("IP地址", l.getIpAddress());
            row.put("状态", l.getStatus() == 1 ? "成功" : "失败");
            row.put("操作时间", l.getOperationTime() != null ? l.getOperationTime().format(DTF) : "");
            row.put("耗时", l.getDuration() + "ms");
            return row;
        }).collect(Collectors.toList());

        ByteArrayOutputStream out = new ByteArrayOutputStream();
        EasyExcel.write(out).sheet("操作日志").doWrite(exportData);
        return out.toByteArray();
    }

    // ========== 异常日志 ==========

    @Override
    public ApiResponse<?> getExceptionLogsPaged(ExceptionLogQueryRequest query) {
        LambdaQueryWrapper<ExceptionLog> wrapper = buildExLogWrapper(query);
        wrapper.orderByDesc(ExceptionLog::getId);

        Page<ExceptionLog> page = exceptionLogMapper.selectPage(
                new Page<>(query.getPage(), query.getPageSize()), wrapper);

        return ApiResponse.successWithTotal(page.getRecords(), (int) page.getTotal());
    }

    @Override
    public ApiResponse<?> getExceptionLogDetail(Long id) {
        ExceptionLog log = exceptionLogMapper.selectById(id);
        if (log == null) return ApiResponse.error("日志不存在", 404);
        return ApiResponse.success(log);
    }

    @Override
    public ApiResponse<?> cleanExceptionLogs(CleanLogRequest request) {
        LambdaQueryWrapper<ExceptionLog> wrapper = new LambdaQueryWrapper<>();
        if (request.getStartTime() != null)
            wrapper.ge(ExceptionLog::getOccurTime, request.getStartTime());
        if (request.getEndTime() != null)
            wrapper.le(ExceptionLog::getOccurTime, request.getEndTime());

        exceptionLogMapper.delete(wrapper);
        return ApiResponse.success(null, "清理成功");
    }

    @Override
    public byte[] exportExceptionLogs(ExceptionLogQueryRequest query) {
        LambdaQueryWrapper<ExceptionLog> wrapper = buildExLogWrapper(query);
        wrapper.orderByDesc(ExceptionLog::getId);
        List<ExceptionLog> logs = exceptionLogMapper.selectList(wrapper);

        List<Map<String, Object>> exportData = logs.stream().map(l -> {
            Map<String, Object> row = new LinkedHashMap<>();
            row.put("ID", l.getId());
            row.put("异常信息", l.getMessage());
            row.put("异常类型", l.getExceptionType());
            row.put("请求地址", l.getRequestUrl());
            row.put("请求方法", l.getRequestMethod());
            row.put("IP地址", l.getIpAddress());
            row.put("操作人", l.getOperator());
            row.put("发生时间", l.getOccurTime() != null ? l.getOccurTime().format(DTF) : "");
            return row;
        }).collect(Collectors.toList());

        ByteArrayOutputStream out = new ByteArrayOutputStream();
        EasyExcel.write(out).sheet("异常日志").doWrite(exportData);
        return out.toByteArray();
    }

    // ========== 私有方法 ==========

    private LambdaQueryWrapper<OperationLog> buildOpLogWrapper(OperationLogQueryRequest query) {
        LambdaQueryWrapper<OperationLog> wrapper = new LambdaQueryWrapper<>();
        if (StringUtils.hasText(query.getOperator()))
            wrapper.like(OperationLog::getOperator, query.getOperator());
        if (StringUtils.hasText(query.getModule()))
            wrapper.like(OperationLog::getModule, query.getModule());
        if (StringUtils.hasText(query.getOperationType()))
            wrapper.like(OperationLog::getOperationType, query.getOperationType());
        if (StringUtils.hasText(query.getIpAddress()))
            wrapper.like(OperationLog::getIpAddress, query.getIpAddress());
        if (query.getStatus() != null)
            wrapper.eq(OperationLog::getStatus, query.getStatus());
        if (query.getStartTime() != null)
            wrapper.ge(OperationLog::getOperationTime, query.getStartTime());
        if (query.getEndTime() != null)
            wrapper.le(OperationLog::getOperationTime, query.getEndTime());
        return wrapper;
    }

    private LambdaQueryWrapper<ExceptionLog> buildExLogWrapper(ExceptionLogQueryRequest query) {
        LambdaQueryWrapper<ExceptionLog> wrapper = new LambdaQueryWrapper<>();
        if (StringUtils.hasText(query.getMessage()))
            wrapper.like(ExceptionLog::getMessage, query.getMessage());
        if (StringUtils.hasText(query.getExceptionType()))
            wrapper.like(ExceptionLog::getExceptionType, query.getExceptionType());
        if (StringUtils.hasText(query.getOperator()))
            wrapper.like(ExceptionLog::getOperator, query.getOperator());
        if (StringUtils.hasText(query.getIpAddress()))
            wrapper.like(ExceptionLog::getIpAddress, query.getIpAddress());
        if (query.getStartTime() != null)
            wrapper.ge(ExceptionLog::getOccurTime, query.getStartTime());
        if (query.getEndTime() != null)
            wrapper.le(ExceptionLog::getOccurTime, query.getEndTime());
        return wrapper;
    }
}
