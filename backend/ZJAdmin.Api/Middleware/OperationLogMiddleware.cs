using System.Diagnostics;
using System.Security.Claims;
using System.Text;
using ZJAdmin.Api.Data;
using ZJAdmin.Api.Models;

namespace ZJAdmin.Api.Middleware;

public class OperationLogMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<OperationLogMiddleware> _logger;

    public OperationLogMiddleware(RequestDelegate next, ILogger<OperationLogMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context, AppDbContext dbContext)
    {
        var path = context.Request.Path.Value ?? "";
        var method = context.Request.Method;

        // Only log API requests
        if (!path.StartsWith("/api/"))
        {
            await _next(context);
            return;
        }

        // Skip GET requests for logs (too noisy)
        if (method == "GET" && path.Contains("/logs/"))
        {
            await _next(context);
            return;
        }

        var sw = Stopwatch.StartNew();

        // Read request body
        var originalBody = context.Request.Body;
        var requestBody = "";
        if (method == "POST" || method == "PUT")
        {
            context.Request.EnableBuffering();
            using var reader = new StreamReader(context.Request.Body, Encoding.UTF8, leaveOpen: true);
            requestBody = await reader.ReadToEndAsync();
            context.Request.Body.Position = 0;
        }

        // Capture response
        var originalResponse = context.Response.Body;
        using var responseBody = new MemoryStream();
        context.Response.Body = responseBody;

        try
        {
            await _next(context);
        }
        finally
        {
            sw.Stop();
            context.Response.Body = originalResponse;
            responseBody.Position = 0;
            var responseText = await new StreamReader(responseBody).ReadToEndAsync();
            responseBody.Position = 0;
            await responseBody.CopyToAsync(originalResponse);

            // Determine module and operation type from path
            var (module, operationType) = ParsePath(path, method);

            var username = context.User?.FindFirst(ClaimTypes.Name)?.Value;

            var opLog = new OperationLog
            {
                Operator = username ?? "匿名",
                Module = module,
                OperationType = operationType,
                RequestUrl = path,
                RequestMethod = method,
                RequestParams = requestBody.Length > 2000 ? requestBody[..2000] : requestBody,
                ResponseResult = responseText.Length > 2000 ? responseText[..2000] : responseText,
                IpAddress = context.Connection.RemoteIpAddress?.ToString(),
                Status = (byte)(context.Response.StatusCode == 200 ? 1 : 0),
                Duration = sw.ElapsedMilliseconds,
                OperationTime = DateTime.UtcNow
            };

            try
            {
                dbContext.OperationLogs.Add(opLog);
                await dbContext.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "保存操作日志失败");
            }
        }
    }

    private static (string module, string operationType) ParsePath(string path, string method)
    {
        var parts = path.Trim('/').Split('/');
        if (parts.Length < 3) return ("其他", "访问");

        var resource = parts[2]; // e.g., "users", "roles", "permissions"
        var module = resource switch
        {
            "auth" => "认证管理",
            "users" => "用户管理",
            "roles" => "角色管理",
            "permissions" => "权限管理",
            "operation-logs" => "操作日志",
            "exception-logs" => "异常日志",
            _ => "其他"
        };

        var operationType = method switch
        {
            "POST" => "新增",
            "PUT" => "修改",
            "DELETE" => "删除",
            "GET" => "查询",
            _ => "其他"
        };

        return (module, operationType);
    }
}
