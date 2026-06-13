using System.Reflection;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;
using ZJAdmin.Api.Attributes;
using ZJAdmin.Api.Services;

namespace ZJAdmin.Api.Middleware;

public class PermissionAuthorizationFilter : IAsyncAuthorizationFilter
{
    private readonly AuthService _authService;

    public PermissionAuthorizationFilter(AuthService authService)
    {
        _authService = authService;
    }

    public async Task OnAuthorizationAsync(AuthorizationFilterContext context)
    {
        if (context.ActionDescriptor is not ControllerActionDescriptor actionDescriptor)
            return;

        var permissionAttr = actionDescriptor.MethodInfo
            .GetCustomAttributes(typeof(PermissionAttribute), true)
            .Cast<PermissionAttribute>()
            .FirstOrDefault();

        if (permissionAttr == null)
        {
            permissionAttr = actionDescriptor.ControllerTypeInfo
                .GetCustomAttributes(typeof(PermissionAttribute), true)
                .Cast<PermissionAttribute>()
                .FirstOrDefault();
            if (permissionAttr == null) return;
        }

        var userIdClaim = context.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier);
        if (userIdClaim == null || !long.TryParse(userIdClaim.Value, out var userId))
        {
            context.Result = new JsonResult(new { Code = 401, Message = "未认证" }) { StatusCode = 401 };
            return;
        }

        var permissions = await _authService.GetUserPermissions(userId);

        if (permissions.Contains("*:*:*"))
            return;

        if (!permissions.Contains(permissionAttr.Code))
        {
            context.Result = new JsonResult(new { Code = 403, Message = "无权限访问" }) { StatusCode = 403 };
        }
    }
}
