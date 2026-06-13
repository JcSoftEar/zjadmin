using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ZJAdmin.Api.Attributes;
using ZJAdmin.Api.DTOs;
using ZJAdmin.Api.Services;

namespace ZJAdmin.Api.Controllers;

[ApiController]
[Route("api/v1/exception-logs")]
[Authorize]
public class ExceptionLogsController : ControllerBase
{
    private readonly LogService _logService;

    public ExceptionLogsController(LogService logService)
    {
        _logService = logService;
    }

    [HttpGet]
    [Permission("logs:exception:query")]
    public async Task<ApiResponse> GetPaged([FromQuery] ExceptionLogQuery query)
    {
        return await _logService.GetExceptionLogsPaged(query);
    }

    [HttpGet("{id}")]
    [Permission("logs:exception:query")]
    public async Task<ApiResponse> GetDetail(long id)
    {
        return await _logService.GetExceptionLogDetail(id);
    }

    [HttpDelete("clean")]
    [Permission("logs:exception:delete")]
    public async Task<ApiResponse> Clean([FromBody] CleanLogRequest request)
    {
        return await _logService.CleanExceptionLogs(request);
    }

    [HttpGet("export")]
    [Permission("logs:exception:query")]
    public async Task<IActionResult> Export([FromQuery] ExceptionLogQuery query)
    {
        var data = await _logService.ExportExceptionLogs(query);
        return File(data, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "异常日志.xlsx");
    }
}
