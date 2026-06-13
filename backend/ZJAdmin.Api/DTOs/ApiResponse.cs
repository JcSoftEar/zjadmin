namespace ZJAdmin.Api.DTOs;

public class ApiResponse
{
    public int Code { get; set; } = 200;
    public string Message { get; set; } = "成功";
    public object? Data { get; set; }
    public int? Total { get; set; }

    public static ApiResponse Success(object? data = null, string message = "成功")
        => new() { Code = 200, Message = message, Data = data };

    public static ApiResponse SuccessWithTotal(object? data, int total, string message = "成功")
        => new() { Code = 200, Message = message, Data = data, Total = total };

    public static ApiResponse Error(string message = "失败", int code = 500)
        => new() { Code = code, Message = message };

    public static ApiResponse Unauthorized(string message = "未认证")
        => new() { Code = 401, Message = message };

    public static ApiResponse Forbidden(string message = "无权限")
        => new() { Code = 403, Message = message };
}

public class PagedRequest
{
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 10;
}

public class LoginRequest
{
    public string Username { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}

public class LoginResponse
{
    public string Token { get; set; } = string.Empty;
    public UserInfo User { get; set; } = null!;
}

public class UserInfo
{
    public long Id { get; set; }
    public string Username { get; set; } = string.Empty;
    public string Nickname { get; set; } = string.Empty;
    public string? Email { get; set; }
    public string? Phone { get; set; }
    public string? Avatar { get; set; }
    public List<string> Roles { get; set; } = new();
    public List<string> Permissions { get; set; } = new();
}

public class UpdateProfileRequest
{
    public string Nickname { get; set; } = string.Empty;
    public string? Email { get; set; }
    public string? Phone { get; set; }
}

public class ChangePasswordRequest
{
    public string OldPassword { get; set; } = string.Empty;
    public string NewPassword { get; set; } = string.Empty;
    public string ConfirmPassword { get; set; } = string.Empty;
}

public class CreateUserRequest
{
    public string Username { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string ConfirmPassword { get; set; } = string.Empty;
    public string Nickname { get; set; } = string.Empty;
    public string? Email { get; set; }
    public string? Phone { get; set; }
    public List<long> RoleIds { get; set; } = new();
    public byte Status { get; set; } = 1;
}

public class UpdateUserRequest
{
    public string Nickname { get; set; } = string.Empty;
    public string? Email { get; set; }
    public string? Phone { get; set; }
    public List<long> RoleIds { get; set; } = new();
    public byte Status { get; set; } = 1;
}

public class CreateRoleRequest
{
    public string Name { get; set; } = string.Empty;
    public string Code { get; set; } = string.Empty;
    public string? Description { get; set; }
}

public class UpdateRoleRequest
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
}

public class CreatePermissionRequest
{
    public long ParentId { get; set; } = 0;
    public string Name { get; set; } = string.Empty;
    public string Code { get; set; } = string.Empty;
    public byte Type { get; set; }
    public string? Path { get; set; }
    public string? Component { get; set; }
    public string? Icon { get; set; }
    public int Sort { get; set; } = 0;
    public byte Visible { get; set; } = 1;
}

public class UpdatePermissionRequest
{
    public long ParentId { get; set; } = 0;
    public string Name { get; set; } = string.Empty;
    public byte Type { get; set; }
    public string? Path { get; set; }
    public string? Component { get; set; }
    public string? Icon { get; set; }
    public int Sort { get; set; } = 0;
    public byte Visible { get; set; } = 1;
}

public class PermissionTreeItem
{
    public long Id { get; set; }
    public long ParentId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Code { get; set; } = string.Empty;
    public byte Type { get; set; }
    public string? Path { get; set; }
    public string? Component { get; set; }
    public string? Icon { get; set; }
    public int Sort { get; set; }
    public byte Visible { get; set; }
    public DateTime CreateTime { get; set; }
    public List<PermissionTreeItem> Children { get; set; } = new();
}

public class RolePermissionsRequest
{
    public List<long> PermissionIds { get; set; } = new();
}

public class OperationLogQuery : PagedRequest
{
    public string? Operator { get; set; }
    public string? Module { get; set; }
    public string? OperationType { get; set; }
    public string? IpAddress { get; set; }
    public byte? Status { get; set; }
    public DateTime? StartTime { get; set; }
    public DateTime? EndTime { get; set; }
}

public class ExceptionLogQuery : PagedRequest
{
    public string? Message { get; set; }
    public string? ExceptionType { get; set; }
    public string? Operator { get; set; }
    public string? IpAddress { get; set; }
    public DateTime? StartTime { get; set; }
    public DateTime? EndTime { get; set; }
}

public class CleanLogRequest
{
    public DateTime? StartTime { get; set; }
    public DateTime? EndTime { get; set; }
}

public class UserQuery : PagedRequest
{
    public string? Username { get; set; }
    public string? Nickname { get; set; }
    public string? Email { get; set; }
    public string? Phone { get; set; }
    public byte? Status { get; set; }
    public long? RoleId { get; set; }
}

public class RoleQuery : PagedRequest
{
    public string? Name { get; set; }
    public string? Code { get; set; }
}
