using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ZJAdmin.Api.Attributes;
using ZJAdmin.Api.DTOs;
using ZJAdmin.Api.Services;

namespace ZJAdmin.Api.Controllers;

[ApiController]
[Route("api/v1/roles")]
[Authorize]
public class RolesController : ControllerBase
{
    private readonly RoleService _roleService;

    public RolesController(RoleService roleService)
    {
        _roleService = roleService;
    }

    [HttpGet]
    [Permission("system:role:query")]
    public async Task<ApiResponse> GetPaged([FromQuery] RoleQuery query)
    {
        return await _roleService.GetPaged(query);
    }

    [HttpGet("all")]
    public async Task<ApiResponse> GetAll()
    {
        return await _roleService.GetAll();
    }

    [HttpPost]
    [Permission("system:role:add")]
    public async Task<ApiResponse> Create([FromBody] CreateRoleRequest request)
    {
        return await _roleService.Create(request);
    }

    [HttpGet("{id}")]
    [Permission("system:role:query")]
    public async Task<ApiResponse> GetById(long id)
    {
        return await _roleService.GetById(id);
    }

    [HttpPut("{id}")]
    [Permission("system:role:edit")]
    public async Task<ApiResponse> Update(long id, [FromBody] UpdateRoleRequest request)
    {
        return await _roleService.Update(id, request);
    }

    [HttpDelete("{id}")]
    [Permission("system:role:delete")]
    public async Task<ApiResponse> Delete(long id)
    {
        return await _roleService.Delete(id);
    }

    [HttpPut("{id}/permissions")]
    [Permission("system:role:edit")]
    public async Task<ApiResponse> AssignPermissions(long id, [FromBody] RolePermissionsRequest request)
    {
        return await _roleService.AssignPermissions(id, request);
    }

    [HttpGet("{id}/permissions")]
    [Permission("system:role:query")]
    public async Task<ApiResponse> GetPermissionIds(long id)
    {
        return await _roleService.GetPermissionIds(id);
    }
}
