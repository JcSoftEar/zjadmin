using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ZJAdmin.Api.Attributes;
using ZJAdmin.Api.DTOs;
using ZJAdmin.Api.Services;

namespace ZJAdmin.Api.Controllers;

[ApiController]
[Route("api/v1/permissions")]
[Authorize]
public class PermissionsController : ControllerBase
{
    private readonly PermissionService _permissionService;

    public PermissionsController(PermissionService permissionService)
    {
        _permissionService = permissionService;
    }

    [HttpGet("tree")]
    [Permission("system:permission:query")]
    public async Task<ApiResponse> GetTree()
    {
        return await _permissionService.GetTree();
    }

    [HttpPost]
    [Permission("system:permission:add")]
    public async Task<ApiResponse> Create([FromBody] CreatePermissionRequest request)
    {
        return await _permissionService.Create(request);
    }

    [HttpGet("{id}")]
    [Permission("system:permission:query")]
    public async Task<ApiResponse> GetById(long id)
    {
        return await _permissionService.GetById(id);
    }

    [HttpPut("{id}")]
    [Permission("system:permission:edit")]
    public async Task<ApiResponse> Update(long id, [FromBody] UpdatePermissionRequest request)
    {
        return await _permissionService.Update(id, request);
    }

    [HttpDelete("{id}")]
    [Permission("system:permission:delete")]
    public async Task<ApiResponse> Delete(long id)
    {
        return await _permissionService.Delete(id);
    }
}
