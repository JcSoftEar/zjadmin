using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ZJAdmin.Api.Attributes;
using ZJAdmin.Api.DTOs;
using ZJAdmin.Api.Services;

namespace ZJAdmin.Api.Controllers;

[ApiController]
[Route("api/v1/operation-logs")]
[Authorize]
public class OperationLogsController : ControllerBase
{
    private readonly LogService _logService;

    public OperationLogsController(LogService logService)
    {
        _logService = logService;
    }

    [HttpGet]
    [Permission("logs:operation:query")]
    public async Task<ApiResponse> GetPaged([FromQuery] OperationLogQuery query)
    {
        return await _logService.GetOperationLogsPaged(query);
    }

    [HttpGet("{id}")]
    [Permission("logs:operation:query")]
    public async Task<ApiResponse> GetDetail(long id)
    {
        return await _logService.GetOperationLogDetail(id);
    }

    [HttpDelete("clean")]
    [Permission("logs:operation:delete")]
    public async Task<ApiResponse> Clean([FromBody] CleanLogRequest request)
    {
        return await _logService.CleanOperationLogs(request);
    }

    [HttpGet("export")]
    [Permission("logs:operation:query")]
    public async Task<IActionResult> Export([FromQuery] OperationLogQuery query)
    {
        var data = await _logService.ExportOperationLogs(query);
        return File(data, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "操作日志.xlsx");
    }
}
