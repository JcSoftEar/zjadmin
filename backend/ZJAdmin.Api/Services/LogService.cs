using Microsoft.EntityFrameworkCore;
using MiniExcelLibs;
using ZJAdmin.Api.Data;
using ZJAdmin.Api.DTOs;
using ZJAdmin.Api.Models;

namespace ZJAdmin.Api.Services;

public class LogService
{
    private readonly AppDbContext _db;

    public LogService(AppDbContext db)
    {
        _db = db;
    }

    // Operation logs
    public async Task<ApiResponse> GetOperationLogsPaged(OperationLogQuery query)
    {
        var q = _db.OperationLogs.AsQueryable();

        if (!string.IsNullOrWhiteSpace(query.Operator))
            q = q.Where(l => l.Operator != null && l.Operator.Contains(query.Operator));
        if (!string.IsNullOrWhiteSpace(query.Module))
            q = q.Where(l => l.Module != null && l.Module.Contains(query.Module));
        if (!string.IsNullOrWhiteSpace(query.OperationType))
            q = q.Where(l => l.OperationType != null && l.OperationType.Contains(query.OperationType));
        if (!string.IsNullOrWhiteSpace(query.IpAddress))
            q = q.Where(l => l.IpAddress != null && l.IpAddress.Contains(query.IpAddress));
        if (query.Status.HasValue)
            q = q.Where(l => l.Status == query.Status.Value);
        if (query.StartTime.HasValue)
            q = q.Where(l => l.OperationTime >= query.StartTime.Value);
        if (query.EndTime.HasValue)
            q = q.Where(l => l.OperationTime <= query.EndTime.Value);

        var total = await q.CountAsync();
        var logs = await q
            .OrderByDescending(l => l.Id)
            .Skip((query.Page - 1) * query.PageSize)
            .Take(query.PageSize)
            .ToListAsync();

        return ApiResponse.SuccessWithTotal(logs, total);
    }

    public async Task<ApiResponse> GetOperationLogDetail(long id)
    {
        var log = await _db.OperationLogs.FindAsync(id);
        if (log == null)
            return ApiResponse.Error("日志不存在", 404);

        return ApiResponse.Success(log);
    }

    public async Task<ApiResponse> CleanOperationLogs(CleanLogRequest request)
    {
        var q = _db.OperationLogs.AsQueryable();
        if (request.StartTime.HasValue)
            q = q.Where(l => l.OperationTime >= request.StartTime.Value);
        if (request.EndTime.HasValue)
            q = q.Where(l => l.OperationTime <= request.EndTime.Value);

        _db.OperationLogs.RemoveRange(await q.ToListAsync());
        await _db.SaveChangesAsync();

        return ApiResponse.Success(null, "清理成功");
    }

    public async Task<byte[]> ExportOperationLogs(OperationLogQuery query)
    {
        var q = _db.OperationLogs.AsQueryable();

        if (!string.IsNullOrWhiteSpace(query.Operator))
            q = q.Where(l => l.Operator != null && l.Operator.Contains(query.Operator));
        if (!string.IsNullOrWhiteSpace(query.Module))
            q = q.Where(l => l.Module != null && l.Module.Contains(query.Module));
        if (query.Status.HasValue)
            q = q.Where(l => l.Status == query.Status.Value);
        if (query.StartTime.HasValue)
            q = q.Where(l => l.OperationTime >= query.StartTime.Value);
        if (query.EndTime.HasValue)
            q = q.Where(l => l.OperationTime <= query.EndTime.Value);

        var logs = await q.OrderByDescending(l => l.Id).ToListAsync();

        var exportData = logs.Select(l => new
        {
            l.Id,
            l.Operator,
            l.Module,
            操作类型 = l.OperationType,
            请求地址 = l.RequestUrl,
            请求方法 = l.RequestMethod,
            l.IpAddress,
            状态 = l.Status == 1 ? "成功" : "失败",
            操作时间 = l.OperationTime.ToString("yyyy-MM-dd HH:mm:ss"),
            耗时 = $"{l.Duration}ms"
        }).ToList();

        using var stream = new MemoryStream();
        await MiniExcel.SaveAsAsync(stream, exportData);
        return stream.ToArray();
    }

    // Exception logs
    public async Task<ApiResponse> GetExceptionLogsPaged(ExceptionLogQuery query)
    {
        var q = _db.ExceptionLogs.AsQueryable();

        if (!string.IsNullOrWhiteSpace(query.Message))
            q = q.Where(l => l.Message != null && l.Message.Contains(query.Message));
        if (!string.IsNullOrWhiteSpace(query.ExceptionType))
            q = q.Where(l => l.ExceptionType != null && l.ExceptionType.Contains(query.ExceptionType));
        if (!string.IsNullOrWhiteSpace(query.Operator))
            q = q.Where(l => l.Operator != null && l.Operator.Contains(query.Operator));
        if (!string.IsNullOrWhiteSpace(query.IpAddress))
            q = q.Where(l => l.IpAddress != null && l.IpAddress.Contains(query.IpAddress));
        if (query.StartTime.HasValue)
            q = q.Where(l => l.OccurTime >= query.StartTime.Value);
        if (query.EndTime.HasValue)
            q = q.Where(l => l.OccurTime <= query.EndTime.Value);

        var total = await q.CountAsync();
        var logs = await q
            .OrderByDescending(l => l.Id)
            .Skip((query.Page - 1) * query.PageSize)
            .Take(query.PageSize)
            .ToListAsync();

        return ApiResponse.SuccessWithTotal(logs, total);
    }

    public async Task<ApiResponse> GetExceptionLogDetail(long id)
    {
        var log = await _db.ExceptionLogs.FindAsync(id);
        if (log == null)
            return ApiResponse.Error("日志不存在", 404);

        return ApiResponse.Success(log);
    }

    public async Task<ApiResponse> CleanExceptionLogs(CleanLogRequest request)
    {
        var q = _db.ExceptionLogs.AsQueryable();
        if (request.StartTime.HasValue)
            q = q.Where(l => l.OccurTime >= request.StartTime.Value);
        if (request.EndTime.HasValue)
            q = q.Where(l => l.OccurTime <= request.EndTime.Value);

        _db.ExceptionLogs.RemoveRange(await q.ToListAsync());
        await _db.SaveChangesAsync();

        return ApiResponse.Success(null, "清理成功");
    }

    public async Task<byte[]> ExportExceptionLogs(ExceptionLogQuery query)
    {
        var q = _db.ExceptionLogs.AsQueryable();

        if (!string.IsNullOrWhiteSpace(query.Message))
            q = q.Where(l => l.Message != null && l.Message.Contains(query.Message));
        if (!string.IsNullOrWhiteSpace(query.ExceptionType))
            q = q.Where(l => l.ExceptionType != null && l.ExceptionType.Contains(query.ExceptionType));
        if (query.StartTime.HasValue)
            q = q.Where(l => l.OccurTime >= query.StartTime.Value);
        if (query.EndTime.HasValue)
            q = q.Where(l => l.OccurTime <= query.EndTime.Value);

        var logs = await q.OrderByDescending(l => l.Id).ToListAsync();

        var exportData = logs.Select(l => new
        {
            l.Id,
            异常信息 = l.Message,
            异常类型 = l.ExceptionType,
            请求地址 = l.RequestUrl,
            请求方法 = l.RequestMethod,
            l.IpAddress,
            l.Operator,
            发生时间 = l.OccurTime.ToString("yyyy-MM-dd HH:mm:ss")
        }).ToList();

        using var stream = new MemoryStream();
        await MiniExcel.SaveAsAsync(stream, exportData);
        return stream.ToArray();
    }
}
