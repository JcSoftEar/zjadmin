using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ZJAdmin.Api.Attributes;
using ZJAdmin.Api.DTOs;
using ZJAdmin.Api.Services;

namespace ZJAdmin.Api.Controllers;

[ApiController]
[Route("api/v1/users")]
[Authorize]
public class UsersController : ControllerBase
{
    private readonly UserService _userService;

    public UsersController(UserService userService)
    {
        _userService = userService;
    }

    [HttpGet]
    [Permission("system:user:query")]
    public async Task<ApiResponse> GetPaged([FromQuery] UserQuery query)
    {
        return await _userService.GetPaged(query);
    }

    [HttpGet("{id}")]
    [Permission("system:user:query")]
    public async Task<ApiResponse> GetById(long id)
    {
        return await _userService.GetById(id);
    }

    [HttpPost]
    [Permission("system:user:add")]
    public async Task<ApiResponse> Create([FromBody] CreateUserRequest request)
    {
        return await _userService.Create(request);
    }

    [HttpPut("{id}")]
    [Permission("system:user:edit")]
    public async Task<ApiResponse> Update(long id, [FromBody] UpdateUserRequest request)
    {
        return await _userService.Update(id, request);
    }

    [HttpDelete("{id}")]
    [Permission("system:user:delete")]
    public async Task<ApiResponse> Delete(long id)
    {
        return await _userService.Delete(id);
    }

    [HttpPut("{id}/reset-password")]
    [Permission("system:user:edit")]
    public async Task<ApiResponse> ResetPassword(long id)
    {
        return await _userService.ResetPassword(id);
    }

    [HttpPut("{id}/status")]
    [Permission("system:user:edit")]
    public async Task<ApiResponse> SetStatus(long id, [FromBody] byte status)
    {
        return await _userService.SetStatus(id, status);
    }

    private long GetUserId()
    {
        var claim = User.FindFirst(ClaimTypes.NameIdentifier);
        return long.Parse(claim?.Value ?? "0");
    }
}
