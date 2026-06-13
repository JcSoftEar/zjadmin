using Microsoft.EntityFrameworkCore;
using ZJAdmin.Api.Data;
using ZJAdmin.Api.DTOs;
using ZJAdmin.Api.Models;

namespace ZJAdmin.Api.Services;

public class UserService
{
    private readonly AppDbContext _db;

    public UserService(AppDbContext db)
    {
        _db = db;
    }

    public async Task<ApiResponse> GetPaged(UserQuery query)
    {
        var q = _db.Users.Where(u => u.IsDeleted == 0).AsQueryable();

        if (!string.IsNullOrWhiteSpace(query.Username))
            q = q.Where(u => u.Username.Contains(query.Username));
        if (!string.IsNullOrWhiteSpace(query.Nickname))
            q = q.Where(u => u.Nickname.Contains(query.Nickname));
        if (!string.IsNullOrWhiteSpace(query.Email))
            q = q.Where(u => u.Email != null && u.Email.Contains(query.Email));
        if (!string.IsNullOrWhiteSpace(query.Phone))
            q = q.Where(u => u.Phone != null && u.Phone.Contains(query.Phone));
        if (query.Status.HasValue)
            q = q.Where(u => u.Status == query.Status.Value);
        if (query.RoleId.HasValue)
            q = q.Where(u => u.UserRoles.Any(ur => ur.RoleId == query.RoleId.Value));

        var total = await q.CountAsync();
        var users = await q
            .Include(u => u.UserRoles).ThenInclude(ur => ur.Role)
            .OrderByDescending(u => u.Id)
            .Skip((query.Page - 1) * query.PageSize)
            .Take(query.PageSize)
            .ToListAsync();

        var data = users.Select(u => new
        {
            u.Id,
            u.Username,
            u.Nickname,
            u.Email,
            u.Phone,
            u.Status,
            Roles = u.UserRoles.Select(ur => new { ur.Role.Id, ur.Role.Name, ur.Role.Code }),
            u.CreateTime,
            u.UpdateTime
        });

        return ApiResponse.SuccessWithTotal(data, total);
    }

    public async Task<ApiResponse> GetById(long id)
    {
        var user = await _db.Users
            .Include(u => u.UserRoles)
            .FirstOrDefaultAsync(u => u.Id == id && u.IsDeleted == 0);

        if (user == null)
            return ApiResponse.Error("用户不存在", 404);

        return ApiResponse.Success(new
        {
            user.Id,
            user.Username,
            user.Nickname,
            user.Email,
            user.Phone,
            user.Status,
            RoleIds = user.UserRoles.Select(ur => ur.RoleId).ToList(),
            user.CreateTime
        });
    }

    public async Task<ApiResponse> Create(CreateUserRequest request)
    {
        if (await _db.Users.AnyAsync(u => u.Username == request.Username))
            return ApiResponse.Error("用户名已存在", 400);

        if (request.Password != request.ConfirmPassword)
            return ApiResponse.Error("两次密码不一致", 400);

        var user = new User
        {
            Username = request.Username,
            Password = BCrypt.Net.BCrypt.HashPassword(request.Password),
            Nickname = request.Nickname,
            Email = request.Email,
            Phone = request.Phone,
            Status = request.Status,
            CreateTime = DateTime.UtcNow
        };

        _db.Users.Add(user);
        await _db.SaveChangesAsync();

        // Assign roles
        if (request.RoleIds.Any())
        {
            foreach (var roleId in request.RoleIds)
            {
                _db.UserRoles.Add(new UserRole { UserId = user.Id, RoleId = roleId });
            }
            await _db.SaveChangesAsync();
        }

        return ApiResponse.Success(new { user.Id }, "创建成功");
    }

    public async Task<ApiResponse> Update(long id, UpdateUserRequest request)
    {
        var user = await _db.Users
            .Include(u => u.UserRoles)
            .FirstOrDefaultAsync(u => u.Id == id && u.IsDeleted == 0);

        if (user == null)
            return ApiResponse.Error("用户不存在", 404);

        user.Nickname = request.Nickname;
        user.Email = request.Email;
        user.Phone = request.Phone;
        user.Status = request.Status;
        user.UpdateTime = DateTime.UtcNow;

        // Update roles
        _db.UserRoles.RemoveRange(user.UserRoles);
        if (request.RoleIds.Any())
        {
            foreach (var roleId in request.RoleIds)
            {
                _db.UserRoles.Add(new UserRole { UserId = id, RoleId = roleId });
            }
        }

        await _db.SaveChangesAsync();
        return ApiResponse.Success(null, "更新成功");
    }

    public async Task<ApiResponse> Delete(long id)
    {
        var user = await _db.Users.FindAsync(id);
        if (user == null)
            return ApiResponse.Error("用户不存在", 404);

        if (user.Username == "admin")
            return ApiResponse.Error("不可删除超级管理员", 400);

        user.IsDeleted = 1;
        user.UpdateTime = DateTime.UtcNow;
        await _db.SaveChangesAsync();

        return ApiResponse.Success(null, "删除成功");
    }

    public async Task<ApiResponse> ResetPassword(long id)
    {
        var user = await _db.Users.FindAsync(id);
        if (user == null)
            return ApiResponse.Error("用户不存在", 404);

        user.Password = BCrypt.Net.BCrypt.HashPassword("123456");
        user.UpdateTime = DateTime.UtcNow;
        await _db.SaveChangesAsync();

        return ApiResponse.Success(null, "密码已重置为 123456");
    }

    public async Task<ApiResponse> SetStatus(long id, byte status)
    {
        var user = await _db.Users.FindAsync(id);
        if (user == null)
            return ApiResponse.Error("用户不存在", 404);

        user.Status = status;
        user.UpdateTime = DateTime.UtcNow;
        await _db.SaveChangesAsync();

        return ApiResponse.Success(null, status == 1 ? "已启用" : "已禁用");
    }

    public async Task<ApiResponse> ChangePassword(long userId, ChangePasswordRequest request)
    {
        var user = await _db.Users.FindAsync(userId);
        if (user == null)
            return ApiResponse.Error("用户不存在", 404);

        if (!BCrypt.Net.BCrypt.Verify(request.OldPassword, user.Password))
            return ApiResponse.Error("原密码错误", 400);

        if (request.NewPassword != request.ConfirmPassword)
            return ApiResponse.Error("两次密码不一致", 400);

        user.Password = BCrypt.Net.BCrypt.HashPassword(request.NewPassword);
        user.UpdateTime = DateTime.UtcNow;
        await _db.SaveChangesAsync();

        return ApiResponse.Success(null, "密码修改成功");
    }

    public async Task<ApiResponse> UpdateProfile(long userId, UpdateProfileRequest request)
    {
        var user = await _db.Users.FindAsync(userId);
        if (user == null)
            return ApiResponse.Error("用户不存在", 404);

        user.Nickname = request.Nickname;
        user.Email = request.Email;
        user.Phone = request.Phone;
        user.UpdateTime = DateTime.UtcNow;
        await _db.SaveChangesAsync();

        return ApiResponse.Success(null, "更新成功");
    }
}
