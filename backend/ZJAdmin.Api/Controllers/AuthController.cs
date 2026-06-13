using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ZJAdmin.Api.DTOs;
using ZJAdmin.Api.Services;

namespace ZJAdmin.Api.Controllers;

[ApiController]
[Route("api/v1/auth")]
public class AuthController : ControllerBase
{
    private readonly AuthService _authService;
    private readonly UserService _userService;

    public AuthController(AuthService authService, UserService userService)
    {
        _authService = authService;
        _userService = userService;
    }

    [HttpPost("login")]
    [AllowAnonymous]
    public async Task<ApiResponse> Login([FromBody] LoginRequest request)
    {
        return await _authService.Login(request);
    }

    [HttpPost("logout")]
    public ApiResponse Logout()
    {
        return ApiResponse.Success(null, "退出成功");
    }

    [HttpGet("profile")]
    public async Task<ApiResponse> GetProfile()
    {
        var userId = GetUserId();
        var user = await _authService.GetCurrentUser(userId);
        if (user == null)
            return ApiResponse.Error("用户不存在", 404);
        return ApiResponse.Success(user);
    }

    [HttpPut("profile")]
    public async Task<ApiResponse> UpdateProfile([FromBody] UpdateProfileRequest request)
    {
        var userId = GetUserId();
        return await _userService.UpdateProfile(userId, request);
    }

    [HttpPut("password")]
    public async Task<ApiResponse> ChangePassword([FromBody] ChangePasswordRequest request)
    {
        var userId = GetUserId();
        return await _userService.ChangePassword(userId, request);
    }

    private long GetUserId()
    {
        var claim = User.FindFirst(ClaimTypes.NameIdentifier);
        return long.Parse(claim?.Value ?? "0");
    }
}
