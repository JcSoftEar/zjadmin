using Microsoft.EntityFrameworkCore;
using ZJAdmin.Api.Data;
using ZJAdmin.Api.DTOs;
using ZJAdmin.Api.Models;

namespace ZJAdmin.Api.Services;

public class RoleService
{
    private readonly AppDbContext _db;

    public RoleService(AppDbContext db)
    {
        _db = db;
    }

    public async Task<ApiResponse> GetPaged(RoleQuery query)
    {
        var q = _db.Roles.AsQueryable();

        if (!string.IsNullOrWhiteSpace(query.Name))
            q = q.Where(r => r.Name.Contains(query.Name));
        if (!string.IsNullOrWhiteSpace(query.Code))
            q = q.Where(r => r.Code.Contains(query.Code));

        var total = await q.CountAsync();
        var roles = await q
            .OrderByDescending(r => r.Id)
            .Skip((query.Page - 1) * query.PageSize)
            .Take(query.PageSize)
            .ToListAsync();

        return ApiResponse.SuccessWithTotal(roles, total);
    }

    public async Task<ApiResponse> GetAll()
    {
        var roles = await _db.Roles.OrderBy(r => r.Id).ToListAsync();
        return ApiResponse.Success(roles);
    }

    public async Task<ApiResponse> Create(CreateRoleRequest request)
    {
        if (await _db.Roles.AnyAsync(r => r.Code == request.Code))
            return ApiResponse.Error("角色标识已存在", 400);

        var role = new Role
        {
            Name = request.Name,
            Code = request.Code,
            Description = request.Description,
            CreateTime = DateTime.UtcNow
        };

        _db.Roles.Add(role);
        await _db.SaveChangesAsync();

        return ApiResponse.Success(new { role.Id }, "创建成功");
    }

    public async Task<ApiResponse> GetById(long id)
    {
        var role = await _db.Roles.FindAsync(id);
        if (role == null)
            return ApiResponse.Error("角色不存在", 404);

        return ApiResponse.Success(role);
    }

    public async Task<ApiResponse> Update(long id, UpdateRoleRequest request)
    {
        var role = await _db.Roles.FindAsync(id);
        if (role == null)
            return ApiResponse.Error("角色不存在", 404);

        role.Name = request.Name;
        role.Description = request.Description;
        role.UpdateTime = DateTime.UtcNow;
        await _db.SaveChangesAsync();

        return ApiResponse.Success(null, "更新成功");
    }

    public async Task<ApiResponse> Delete(long id)
    {
        var role = await _db.Roles.FindAsync(id);
        if (role == null)
            return ApiResponse.Error("角色不存在", 404);

        if (role.Code == "super_admin")
            return ApiResponse.Error("不可删除超级管理员角色", 400);

        var userCount = await _db.UserRoles.CountAsync(ur => ur.RoleId == id);
        if (userCount > 0)
            return ApiResponse.Error($"该角色下还有 {userCount} 个用户，无法删除", 400);

        _db.Roles.Remove(role);
        await _db.SaveChangesAsync();

        return ApiResponse.Success(null, "删除成功");
    }

    public async Task<ApiResponse> AssignPermissions(long roleId, RolePermissionsRequest request)
    {
        var role = await _db.Roles.FindAsync(roleId);
        if (role == null)
            return ApiResponse.Error("角色不存在", 404);

        var existing = await _db.RolePermissions.Where(rp => rp.RoleId == roleId).ToListAsync();
        _db.RolePermissions.RemoveRange(existing);

        foreach (var permissionId in request.PermissionIds)
        {
            _db.RolePermissions.Add(new RolePermission { RoleId = roleId, PermissionId = permissionId });
        }

        await _db.SaveChangesAsync();
        return ApiResponse.Success(null, "权限分配成功");
    }

    public async Task<ApiResponse> GetPermissionIds(long roleId)
    {
        if (!await _db.Roles.AnyAsync(r => r.Id == roleId))
            return ApiResponse.Error("角色不存在", 404);

        var permissionIds = await _db.RolePermissions
            .Where(rp => rp.RoleId == roleId)
            .Select(rp => rp.PermissionId)
            .ToListAsync();

        return ApiResponse.Success(permissionIds);
    }
}
