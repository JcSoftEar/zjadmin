using Microsoft.EntityFrameworkCore;
using ZJAdmin.Api.Data;
using ZJAdmin.Api.DTOs;
using ZJAdmin.Api.Models;

namespace ZJAdmin.Api.Services;

public class PermissionService
{
    private readonly AppDbContext _db;

    public PermissionService(AppDbContext db)
    {
        _db = db;
    }

    public async Task<ApiResponse> GetTree()
    {
        var permissions = await _db.Permissions.OrderBy(p => p.Sort).ToListAsync();
        var tree = BuildTree(permissions, 0);
        return ApiResponse.Success(tree);
    }

    public async Task<ApiResponse> Create(CreatePermissionRequest request)
    {
        if (await _db.Permissions.AnyAsync(p => p.Code == request.Code))
            return ApiResponse.Error("权限标识已存在", 400);

        var permission = new Permission
        {
            ParentId = request.ParentId,
            Name = request.Name,
            Code = request.Code,
            Type = request.Type,
            Path = request.Path,
            Component = request.Component,
            Icon = request.Icon,
            Sort = request.Sort,
            Visible = request.Visible,
            CreateTime = DateTime.UtcNow
        };

        _db.Permissions.Add(permission);
        await _db.SaveChangesAsync();

        return ApiResponse.Success(new { permission.Id }, "创建成功");
    }

    public async Task<ApiResponse> GetById(long id)
    {
        var permission = await _db.Permissions.FindAsync(id);
        if (permission == null)
            return ApiResponse.Error("权限不存在", 404);

        return ApiResponse.Success(permission);
    }

    public async Task<ApiResponse> Update(long id, UpdatePermissionRequest request)
    {
        var permission = await _db.Permissions.FindAsync(id);
        if (permission == null)
            return ApiResponse.Error("权限不存在", 404);

        permission.ParentId = request.ParentId;
        permission.Name = request.Name;
        permission.Type = request.Type;
        permission.Path = request.Path;
        permission.Component = request.Component;
        permission.Icon = request.Icon;
        permission.Sort = request.Sort;
        permission.Visible = request.Visible;
        permission.UpdateTime = DateTime.UtcNow;
        await _db.SaveChangesAsync();

        return ApiResponse.Success(null, "更新成功");
    }

    public async Task<ApiResponse> Delete(long id)
    {
        var permission = await _db.Permissions.FindAsync(id);
        if (permission == null)
            return ApiResponse.Error("权限不存在", 404);

        // Check if has children
        var hasChildren = await _db.Permissions.AnyAsync(p => p.ParentId == id);
        if (hasChildren)
            return ApiResponse.Error("该权限下有子权限，无法删除", 400);

        // Check if assigned to any role
        var roleCount = await _db.RolePermissions.CountAsync(rp => rp.PermissionId == id);
        if (roleCount > 0)
            return ApiResponse.Error($"该权限已分配给 {roleCount} 个角色，无法删除", 400);

        _db.Permissions.Remove(permission);
        await _db.SaveChangesAsync();

        return ApiResponse.Success(null, "删除成功");
    }

    private static List<PermissionTreeItem> BuildTree(List<Permission> permissions, long parentId)
    {
        return permissions
            .Where(p => p.ParentId == parentId)
            .OrderBy(p => p.Sort)
            .Select(p => new PermissionTreeItem
            {
                Id = p.Id,
                ParentId = p.ParentId,
                Name = p.Name,
                Code = p.Code,
                Type = p.Type,
                Path = p.Path,
                Component = p.Component,
                Icon = p.Icon,
                Sort = p.Sort,
                Visible = p.Visible,
                CreateTime = p.CreateTime,
                Children = BuildTree(permissions, p.Id)
            })
            .ToList();
    }
}
