using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using ZJAdmin.Api.Data;
using ZJAdmin.Api.DTOs;
using ZJAdmin.Api.Models;

namespace ZJAdmin.Api.Services;

public class AuthService
{
    private readonly AppDbContext _db;
    private readonly IConfiguration _config;
    private static readonly Dictionary<string, LoginAttempt> LoginAttempts = new();

    public AuthService(AppDbContext db, IConfiguration config)
    {
        _db = db;
        _config = config;
    }

    public async Task<ApiResponse> Login(LoginRequest request)
    {
        // Check login lockout
        var lockoutKey = $"login_{request.Username}";
        if (LoginAttempts.TryGetValue(lockoutKey, out var attempt) && attempt.IsLocked())
        {
            var remaining = (attempt.LockoutEnd - DateTime.UtcNow)?.Minutes ?? 0;
            return ApiResponse.Error($"账户已锁定，请 {remaining} 分钟后重试", 400);
        }

        var user = await _db.Users
            .Include(u => u.UserRoles)
                .ThenInclude(ur => ur.Role)
            .FirstOrDefaultAsync(u => u.Username == request.Username && u.IsDeleted == 0);

        if (user == null || !BCrypt.Net.BCrypt.Verify(request.Password, user.Password))
        {
            // Record failed attempt
            if (LoginAttempts.TryGetValue(lockoutKey, out var existing))
            {
                existing.Count++;
                existing.LastAttempt = DateTime.UtcNow;
                if (existing.Count >= 5)
                {
                    existing.LockoutEnd = DateTime.UtcNow.AddMinutes(15);
                }
            }
            else
            {
                LoginAttempts[lockoutKey] = new LoginAttempt { Count = 1, LastAttempt = DateTime.UtcNow };
            }

            // Log failed login as exception log for security auditing
            try
            {
                _db.ExceptionLogs.Add(new Models.ExceptionLog
                {
                    Message = "登录失败：用户名或密码错误",
                    ExceptionType = "LoginFailed",
                    RequestUrl = "/api/v1/auth/login",
                    RequestMethod = "POST",
                    RequestParams = $"{{\"username\":\"{request.Username}\"}}",
                    IpAddress = null, // Will be set from HttpContext where available
                    Operator = request.Username,
                    OccurTime = DateTime.UtcNow
                });
                await _db.SaveChangesAsync();
            }
            catch { /* fail silently */ }

            return ApiResponse.Error("用户名或密码错误", 400);
        }

        if (user.Status == 0)
        {
            return ApiResponse.Error("账户已被禁用", 400);
        }

        // Clear login attempts on success
        LoginAttempts.Remove(lockoutKey);
        user.LoginFailCount = 0;
        user.LockoutEndTime = null;

        var token = GenerateToken(user);
        var permissions = await GetUserPermissions(user.Id);

        var roleCodes = user.UserRoles.Select(ur => ur.Role.Code).ToList();

        return ApiResponse.Success(new LoginResponse
        {
            Token = token,
            User = new UserInfo
            {
                Id = user.Id,
                Username = user.Username,
                Nickname = user.Nickname,
                Email = user.Email,
                Phone = user.Phone,
                Roles = roleCodes,
                Permissions = permissions
            }
        }, "登录成功");
    }

    public async Task<UserInfo?> GetCurrentUser(long userId)
    {
        var user = await _db.Users
            .Include(u => u.UserRoles)
                .ThenInclude(ur => ur.Role)
            .FirstOrDefaultAsync(u => u.Id == userId && u.IsDeleted == 0);

        if (user == null || user.Status == 0) return null;

        var permissions = await GetUserPermissions(userId);
        var roleCodes = user.UserRoles.Select(ur => ur.Role.Code).ToList();

        return new UserInfo
        {
            Id = user.Id,
            Username = user.Username,
            Nickname = user.Nickname,
            Email = user.Email,
            Phone = user.Phone,
            Roles = roleCodes,
            Permissions = permissions
        };
    }

    public async Task<List<string>> GetUserPermissions(long userId)
    {
        var isSuperAdmin = await _db.UserRoles
            .AnyAsync(ur => ur.UserId == userId && ur.Role.Code == "super_admin");

        if (isSuperAdmin)
        {
            return new List<string> { "*:*:*" };
        }

        var permissions = await _db.UserRoles
            .Where(ur => ur.UserId == userId)
            .SelectMany(ur => ur.Role.RolePermissions)
            .Select(rp => rp.Permission.Code)
            .Distinct()
            .ToListAsync();

        return permissions;
    }

    private string GenerateToken(User user)
    {
        var jwtSettings = _config.GetSection("Jwt");
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings["Key"]!));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new(ClaimTypes.Name, user.Username),
        };

        var token = new JwtSecurityToken(
            issuer: jwtSettings["Issuer"],
            audience: jwtSettings["Audience"],
            claims: claims,
            expires: DateTime.UtcNow.AddHours(2),
            signingCredentials: creds
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    private class LoginAttempt
    {
        public int Count { get; set; }
        public DateTime LastAttempt { get; set; }
        public DateTime? LockoutEnd { get; set; }

        public bool IsLocked() => LockoutEnd.HasValue && LockoutEnd > DateTime.UtcNow;
    }
}
