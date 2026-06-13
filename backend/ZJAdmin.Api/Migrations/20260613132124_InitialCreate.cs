using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace ZJAdmin.Api.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "sys_config",
                columns: table => new
                {
                    Id = table.Column<long>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    ConfigKey = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    ConfigValue = table.Column<string>(type: "TEXT", nullable: true),
                    Description = table.Column<string>(type: "TEXT", maxLength: 200, nullable: true),
                    CreateTime = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdateTime = table.Column<DateTime>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_sys_config", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "sys_exception_log",
                columns: table => new
                {
                    Id = table.Column<long>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Message = table.Column<string>(type: "TEXT", nullable: true),
                    ExceptionType = table.Column<string>(type: "TEXT", maxLength: 200, nullable: true),
                    StackTrace = table.Column<string>(type: "TEXT", nullable: true),
                    RequestUrl = table.Column<string>(type: "TEXT", maxLength: 500, nullable: true),
                    RequestMethod = table.Column<string>(type: "TEXT", maxLength: 10, nullable: true),
                    RequestParams = table.Column<string>(type: "TEXT", nullable: true),
                    IpAddress = table.Column<string>(type: "TEXT", maxLength: 50, nullable: true),
                    Operator = table.Column<string>(type: "TEXT", maxLength: 50, nullable: true),
                    OccurTime = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_sys_exception_log", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "sys_operation_log",
                columns: table => new
                {
                    Id = table.Column<long>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Operator = table.Column<string>(type: "TEXT", maxLength: 50, nullable: true),
                    Module = table.Column<string>(type: "TEXT", maxLength: 50, nullable: true),
                    OperationType = table.Column<string>(type: "TEXT", maxLength: 20, nullable: true),
                    RequestUrl = table.Column<string>(type: "TEXT", maxLength: 500, nullable: true),
                    RequestMethod = table.Column<string>(type: "TEXT", maxLength: 10, nullable: true),
                    RequestParams = table.Column<string>(type: "TEXT", nullable: true),
                    ResponseResult = table.Column<string>(type: "TEXT", nullable: true),
                    IpAddress = table.Column<string>(type: "TEXT", maxLength: 50, nullable: true),
                    Status = table.Column<byte>(type: "INTEGER", nullable: false),
                    Duration = table.Column<long>(type: "INTEGER", nullable: false),
                    OperationTime = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_sys_operation_log", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "sys_permission",
                columns: table => new
                {
                    Id = table.Column<long>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    ParentId = table.Column<long>(type: "INTEGER", nullable: false),
                    Name = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    Code = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    Type = table.Column<byte>(type: "INTEGER", nullable: false),
                    Path = table.Column<string>(type: "TEXT", maxLength: 200, nullable: true),
                    Component = table.Column<string>(type: "TEXT", maxLength: 200, nullable: true),
                    Icon = table.Column<string>(type: "TEXT", maxLength: 50, nullable: true),
                    Sort = table.Column<int>(type: "INTEGER", nullable: false),
                    Visible = table.Column<byte>(type: "INTEGER", nullable: false),
                    CreateTime = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdateTime = table.Column<DateTime>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_sys_permission", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "sys_role",
                columns: table => new
                {
                    Id = table.Column<long>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    Code = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    Description = table.Column<string>(type: "TEXT", maxLength: 200, nullable: true),
                    CreateTime = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdateTime = table.Column<DateTime>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_sys_role", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "sys_user",
                columns: table => new
                {
                    Id = table.Column<long>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Username = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    Password = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    Nickname = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    Email = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true),
                    Phone = table.Column<string>(type: "TEXT", maxLength: 20, nullable: true),
                    Status = table.Column<byte>(type: "INTEGER", nullable: false),
                    IsDeleted = table.Column<byte>(type: "INTEGER", nullable: false),
                    CreateTime = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdateTime = table.Column<DateTime>(type: "TEXT", nullable: true),
                    LoginFailCount = table.Column<int>(type: "INTEGER", nullable: false),
                    LockoutEndTime = table.Column<DateTime>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_sys_user", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "sys_role_permission",
                columns: table => new
                {
                    Id = table.Column<long>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    RoleId = table.Column<long>(type: "INTEGER", nullable: false),
                    PermissionId = table.Column<long>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_sys_role_permission", x => x.Id);
                    table.ForeignKey(
                        name: "FK_sys_role_permission_sys_permission_PermissionId",
                        column: x => x.PermissionId,
                        principalTable: "sys_permission",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_sys_role_permission_sys_role_RoleId",
                        column: x => x.RoleId,
                        principalTable: "sys_role",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "sys_user_role",
                columns: table => new
                {
                    Id = table.Column<long>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    UserId = table.Column<long>(type: "INTEGER", nullable: false),
                    RoleId = table.Column<long>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_sys_user_role", x => x.Id);
                    table.ForeignKey(
                        name: "FK_sys_user_role_sys_role_RoleId",
                        column: x => x.RoleId,
                        principalTable: "sys_role",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_sys_user_role_sys_user_UserId",
                        column: x => x.UserId,
                        principalTable: "sys_user",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "sys_config",
                columns: new[] { "Id", "ConfigKey", "ConfigValue", "CreateTime", "Description", "UpdateTime" },
                values: new object[,]
                {
                    { 1L, "site_title", "ZJAdmin 最简后台", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "网站标题", null },
                    { 2L, "site_keywords", "ZJAdmin,最简后台,后台管理,RBAC", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "网站关键词", null },
                    { 3L, "site_icp", "", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "备案信息", null },
                    { 4L, "site_copyright", "Copyright © 2026 ZJAdmin. All rights reserved.", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "版权信息", null }
                });

            migrationBuilder.InsertData(
                table: "sys_permission",
                columns: new[] { "Id", "Code", "Component", "CreateTime", "Icon", "Name", "ParentId", "Path", "Sort", "Type", "UpdateTime", "Visible" },
                values: new object[,]
                {
                    { 1L, "system", "Layout", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Setting", "系统管理", 0L, "/system", 1, (byte)0, null, (byte)1 },
                    { 2L, "system:user", "system/user/index", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "User", "用户管理", 1L, "/system/users", 1, (byte)0, null, (byte)1 },
                    { 3L, "system:role", "system/role/index", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Avatar", "角色管理", 1L, "/system/roles", 2, (byte)0, null, (byte)1 },
                    { 4L, "system:permission", "system/permission/index", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Lock", "权限管理", 1L, "/system/permissions", 3, (byte)0, null, (byte)1 },
                    { 5L, "logs", "Layout", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Document", "日志管理", 0L, "/logs", 2, (byte)0, null, (byte)1 },
                    { 6L, "logs:operation", "logs/operation/index", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Edit", "操作日志", 5L, "/logs/operation", 1, (byte)0, null, (byte)1 },
                    { 7L, "logs:exception", "logs/exception/index", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Warning", "异常日志", 5L, "/logs/exception", 2, (byte)0, null, (byte)1 },
                    { 8L, "dashboard", "dashboard/index", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "HomeFilled", "首页", 0L, "/dashboard", 0, (byte)0, null, (byte)1 },
                    { 9L, "system:user:query", null, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, "查询用户", 2L, null, 0, (byte)1, null, (byte)0 },
                    { 10L, "system:user:add", null, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, "新增用户", 2L, null, 1, (byte)1, null, (byte)0 },
                    { 11L, "system:user:edit", null, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, "编辑用户", 2L, null, 2, (byte)1, null, (byte)0 },
                    { 12L, "system:user:delete", null, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, "删除用户", 2L, null, 3, (byte)1, null, (byte)0 },
                    { 13L, "system:role:query", null, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, "查询角色", 3L, null, 0, (byte)1, null, (byte)0 },
                    { 14L, "system:role:add", null, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, "新增角色", 3L, null, 1, (byte)1, null, (byte)0 },
                    { 15L, "system:role:edit", null, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, "编辑角色", 3L, null, 2, (byte)1, null, (byte)0 },
                    { 16L, "system:role:delete", null, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, "删除角色", 3L, null, 3, (byte)1, null, (byte)0 },
                    { 17L, "system:permission:query", null, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, "查询权限", 4L, null, 0, (byte)1, null, (byte)0 },
                    { 18L, "system:permission:add", null, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, "新增权限", 4L, null, 1, (byte)1, null, (byte)0 },
                    { 19L, "system:permission:edit", null, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, "编辑权限", 4L, null, 2, (byte)1, null, (byte)0 },
                    { 20L, "system:permission:delete", null, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, "删除权限", 4L, null, 3, (byte)1, null, (byte)0 },
                    { 21L, "logs:operation:query", null, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, "查询操作日志", 6L, null, 0, (byte)1, null, (byte)0 },
                    { 22L, "logs:operation:delete", null, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, "删除操作日志", 6L, null, 1, (byte)1, null, (byte)0 },
                    { 23L, "logs:exception:query", null, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, "查询异常日志", 7L, null, 0, (byte)1, null, (byte)0 },
                    { 24L, "logs:exception:delete", null, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, "删除异常日志", 7L, null, 1, (byte)1, null, (byte)0 },
                    { 25L, "system:config", "system/settings/index", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Tools", "系统设置", 1L, "/system/settings", 4, (byte)0, null, (byte)1 },
                    { 26L, "system:config:query", null, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, "查询系统设置", 25L, null, 0, (byte)1, null, (byte)0 },
                    { 27L, "system:config:edit", null, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, "编辑系统设置", 25L, null, 1, (byte)1, null, (byte)0 }
                });

            migrationBuilder.InsertData(
                table: "sys_role",
                columns: new[] { "Id", "Code", "CreateTime", "Description", "Name", "UpdateTime" },
                values: new object[] { 1L, "super_admin", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "系统超级管理员，拥有所有权限", "超级管理员", null });

            migrationBuilder.InsertData(
                table: "sys_user",
                columns: new[] { "Id", "CreateTime", "Email", "IsDeleted", "LockoutEndTime", "LoginFailCount", "Nickname", "Password", "Phone", "Status", "UpdateTime", "Username" },
                values: new object[] { 1L, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "admin@zjadmin.com", (byte)0, null, 0, "超级管理员", "$2a$11$6ND.tr9CxuOn1XDjNBWeJOb81Tx2D6N5IlJexHLcKtLuzuyQ4ONV6", null, (byte)1, null, "admin" });

            migrationBuilder.InsertData(
                table: "sys_role_permission",
                columns: new[] { "Id", "PermissionId", "RoleId" },
                values: new object[,]
                {
                    { 1L, 1L, 1L },
                    { 2L, 2L, 1L },
                    { 3L, 3L, 1L },
                    { 4L, 4L, 1L },
                    { 5L, 5L, 1L },
                    { 6L, 6L, 1L },
                    { 7L, 7L, 1L },
                    { 8L, 8L, 1L },
                    { 9L, 9L, 1L },
                    { 10L, 10L, 1L },
                    { 11L, 11L, 1L },
                    { 12L, 12L, 1L },
                    { 13L, 13L, 1L },
                    { 14L, 14L, 1L },
                    { 15L, 15L, 1L },
                    { 16L, 16L, 1L },
                    { 17L, 17L, 1L },
                    { 18L, 18L, 1L },
                    { 19L, 19L, 1L },
                    { 20L, 20L, 1L },
                    { 21L, 21L, 1L },
                    { 22L, 22L, 1L },
                    { 23L, 23L, 1L },
                    { 24L, 24L, 1L },
                    { 25L, 25L, 1L },
                    { 26L, 26L, 1L },
                    { 27L, 27L, 1L }
                });

            migrationBuilder.InsertData(
                table: "sys_user_role",
                columns: new[] { "Id", "RoleId", "UserId" },
                values: new object[] { 1L, 1L, 1L });

            migrationBuilder.CreateIndex(
                name: "IX_sys_config_ConfigKey",
                table: "sys_config",
                column: "ConfigKey",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_sys_permission_Code",
                table: "sys_permission",
                column: "Code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_sys_role_Code",
                table: "sys_role",
                column: "Code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_sys_role_permission_PermissionId",
                table: "sys_role_permission",
                column: "PermissionId");

            migrationBuilder.CreateIndex(
                name: "IX_sys_role_permission_RoleId_PermissionId",
                table: "sys_role_permission",
                columns: new[] { "RoleId", "PermissionId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_sys_user_Username",
                table: "sys_user",
                column: "Username",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_sys_user_role_RoleId",
                table: "sys_user_role",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "IX_sys_user_role_UserId_RoleId",
                table: "sys_user_role",
                columns: new[] { "UserId", "RoleId" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "sys_config");

            migrationBuilder.DropTable(
                name: "sys_exception_log");

            migrationBuilder.DropTable(
                name: "sys_operation_log");

            migrationBuilder.DropTable(
                name: "sys_role_permission");

            migrationBuilder.DropTable(
                name: "sys_user_role");

            migrationBuilder.DropTable(
                name: "sys_permission");

            migrationBuilder.DropTable(
                name: "sys_role");

            migrationBuilder.DropTable(
                name: "sys_user");
        }
    }
}
