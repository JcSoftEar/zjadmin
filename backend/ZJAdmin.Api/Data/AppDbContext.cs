using Microsoft.EntityFrameworkCore;
using ZJAdmin.Api.Models;

namespace ZJAdmin.Api.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<User> Users => Set<User>();
    public DbSet<Role> Roles => Set<Role>();
    public DbSet<Permission> Permissions => Set<Permission>();
    public DbSet<UserRole> UserRoles => Set<UserRole>();
    public DbSet<RolePermission> RolePermissions => Set<RolePermission>();
    public DbSet<OperationLog> OperationLogs => Set<OperationLog>();
    public DbSet<ExceptionLog> ExceptionLogs => Set<ExceptionLog>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Unique indexes
        modelBuilder.Entity<User>().HasIndex(u => u.Username).IsUnique();
        modelBuilder.Entity<Role>().HasIndex(r => r.Code).IsUnique();
        modelBuilder.Entity<Permission>().HasIndex(p => p.Code).IsUnique();

        // Composite indexes
        modelBuilder.Entity<UserRole>().HasIndex(ur => new { ur.UserId, ur.RoleId }).IsUnique();
        modelBuilder.Entity<RolePermission>().HasIndex(rp => new { rp.RoleId, rp.PermissionId }).IsUnique();

        // Seed data
        SeedData(modelBuilder);
    }

    private static void SeedData(ModelBuilder modelBuilder)
    {
        var now = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        // Permissions
        modelBuilder.Entity<Permission>().HasData(
            // System management
            new Permission { Id = 1, ParentId = 0, Name = "系统管理", Code = "system", Type = 0, Path = "/system", Component = "Layout", Icon = "Setting", Sort = 1, Visible = 1, CreateTime = now },
            new Permission { Id = 2, ParentId = 1, Name = "用户管理", Code = "system:user", Type = 0, Path = "/system/users", Component = "system/user/index", Icon = "User", Sort = 1, Visible = 1, CreateTime = now },
            new Permission { Id = 3, ParentId = 1, Name = "角色管理", Code = "system:role", Type = 0, Path = "/system/roles", Component = "system/role/index", Icon = "Avatar", Sort = 2, Visible = 1, CreateTime = now },
            new Permission { Id = 4, ParentId = 1, Name = "权限管理", Code = "system:permission", Type = 0, Path = "/system/permissions", Component = "system/permission/index", Icon = "Lock", Sort = 3, Visible = 1, CreateTime = now },
            // Log management
            new Permission { Id = 5, ParentId = 0, Name = "日志管理", Code = "logs", Type = 0, Path = "/logs", Component = "Layout", Icon = "Document", Sort = 2, Visible = 1, CreateTime = now },
            new Permission { Id = 6, ParentId = 5, Name = "操作日志", Code = "logs:operation", Type = 0, Path = "/logs/operation", Component = "logs/operation/index", Icon = "Edit", Sort = 1, Visible = 1, CreateTime = now },
            new Permission { Id = 7, ParentId = 5, Name = "异常日志", Code = "logs:exception", Type = 0, Path = "/logs/exception", Component = "logs/exception/index", Icon = "Warning", Sort = 2, Visible = 1, CreateTime = now },
            // Dashboard
            new Permission { Id = 8, ParentId = 0, Name = "首页", Code = "dashboard", Type = 0, Path = "/dashboard", Component = "dashboard/index", Icon = "HomeFilled", Sort = 0, Visible = 1, CreateTime = now },
            // Button permissions
            new Permission { Id = 9,  ParentId = 2, Name = "查询用户", Code = "system:user:query", Type = 1, Sort = 0, Visible = 0, CreateTime = now },
            new Permission { Id = 10, ParentId = 2, Name = "新增用户", Code = "system:user:add", Type = 1, Sort = 1, Visible = 0, CreateTime = now },
            new Permission { Id = 11, ParentId = 2, Name = "编辑用户", Code = "system:user:edit", Type = 1, Sort = 2, Visible = 0, CreateTime = now },
            new Permission { Id = 12, ParentId = 2, Name = "删除用户", Code = "system:user:delete", Type = 1, Sort = 3, Visible = 0, CreateTime = now },
            new Permission { Id = 13, ParentId = 3, Name = "查询角色", Code = "system:role:query", Type = 1, Sort = 0, Visible = 0, CreateTime = now },
            new Permission { Id = 14, ParentId = 3, Name = "新增角色", Code = "system:role:add", Type = 1, Sort = 1, Visible = 0, CreateTime = now },
            new Permission { Id = 15, ParentId = 3, Name = "编辑角色", Code = "system:role:edit", Type = 1, Sort = 2, Visible = 0, CreateTime = now },
            new Permission { Id = 16, ParentId = 3, Name = "删除角色", Code = "system:role:delete", Type = 1, Sort = 3, Visible = 0, CreateTime = now },
            new Permission { Id = 17, ParentId = 4, Name = "查询权限", Code = "system:permission:query", Type = 1, Sort = 0, Visible = 0, CreateTime = now },
            new Permission { Id = 18, ParentId = 4, Name = "新增权限", Code = "system:permission:add", Type = 1, Sort = 1, Visible = 0, CreateTime = now },
            new Permission { Id = 19, ParentId = 4, Name = "编辑权限", Code = "system:permission:edit", Type = 1, Sort = 2, Visible = 0, CreateTime = now },
            new Permission { Id = 20, ParentId = 4, Name = "删除权限", Code = "system:permission:delete", Type = 1, Sort = 3, Visible = 0, CreateTime = now },
            // Log button permissions
            new Permission { Id = 21, ParentId = 6, Name = "查询操作日志", Code = "logs:operation:query", Type = 1, Sort = 0, Visible = 0, CreateTime = now },
            new Permission { Id = 22, ParentId = 6, Name = "删除操作日志", Code = "logs:operation:delete", Type = 1, Sort = 1, Visible = 0, CreateTime = now },
            new Permission { Id = 23, ParentId = 7, Name = "查询异常日志", Code = "logs:exception:query", Type = 1, Sort = 0, Visible = 0, CreateTime = now },
            new Permission { Id = 24, ParentId = 7, Name = "删除异常日志", Code = "logs:exception:delete", Type = 1, Sort = 1, Visible = 0, CreateTime = now }
        );

        // Roles
        modelBuilder.Entity<Role>().HasData(
            new Role { Id = 1, Name = "超级管理员", Code = "super_admin", Description = "系统超级管理员，拥有所有权限", CreateTime = now }
        );

        // Users
        var passwordHash = BCrypt.Net.BCrypt.HashPassword("admin123");
        modelBuilder.Entity<User>().HasData(
            new User { Id = 1, Username = "admin", Password = passwordHash, Nickname = "超级管理员", Email = "admin@zjadmin.com", Status = 1, CreateTime = now }
        );

        // User-Role assignment
        modelBuilder.Entity<UserRole>().HasData(
            new UserRole { Id = 1, UserId = 1, RoleId = 1 }
        );

        // Role-Permission assignment (super_admin gets all permissions 1-24)
        for (long i = 1; i <= 24; i++)
        {
            modelBuilder.Entity<RolePermission>().HasData(
                new RolePermission { Id = i, RoleId = 1, PermissionId = i }
            );
        }
    }
}
