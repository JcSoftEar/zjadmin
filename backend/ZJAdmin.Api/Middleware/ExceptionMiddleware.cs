using System.Net;
using System.Text.Json;
using ZJAdmin.Api.Data;
using ZJAdmin.Api.DTOs;
using ZJAdmin.Api.Models;

namespace ZJAdmin.Api.Middleware;

public class ExceptionMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionMiddleware> _logger;

    public ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context, AppDbContext dbContext)
    {
        try
        {
            await _next(context);
        }
        catch (UnauthorizedAccessException)
        {
            context.Response.StatusCode = 401;
            context.Response.ContentType = "application/json";
            var response = ApiResponse.Unauthorized("未授权访问");
            await context.Response.WriteAsync(JsonSerializer.Serialize(response));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "未处理的异常: {Message}", ex.Message);

            var username = context.User?.FindFirst(System.Security.Claims.ClaimTypes.Name)?.Value;
            var requestBody = "";
            if (context.Request.Method == "POST" || context.Request.Method == "PUT")
            {
                context.Request.EnableBuffering();
                using var reader = new StreamReader(context.Request.Body, System.Text.Encoding.UTF8, leaveOpen: true);
                requestBody = await reader.ReadToEndAsync();
                context.Request.Body.Position = 0;
            }

            var exLog = new ExceptionLog
            {
                Message = ex.Message,
                ExceptionType = ex.GetType().FullName,
                StackTrace = ex.StackTrace,
                RequestUrl = context.Request.Path,
                RequestMethod = context.Request.Method,
                RequestParams = requestBody.Length > 2000 ? requestBody[..2000] : requestBody,
                IpAddress = context.Connection.RemoteIpAddress?.ToString(),
                Operator = username,
                OccurTime = DateTime.UtcNow
            };

            try
            {
                dbContext.ExceptionLogs.Add(exLog);
                await dbContext.SaveChangesAsync();
            }
            catch (Exception dbEx)
            {
                _logger.LogWarning(dbEx, "保存异常日志失败");
            }

            context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
            context.Response.ContentType = "application/json";
            var response = ApiResponse.Error("服务器内部错误");
            await context.Response.WriteAsync(JsonSerializer.Serialize(response));
        }
    }
}
