/* ===========================================================================
 *  ____________/\\\____        ______/\\\_        __/\\\\\\\\\\\\\\\_
 *  __________/\\\\\____        __/\\\\\\\_        _\/////////////\\\_
 *   ________/\\\/\\\____        _\/////\\\_        ____________/\\\/__
 *    ______/\\\/\/\\\____        _____\/\\\_        __________/\\\/____
 *     ____/\\\/__\/\\\____        _____\/\\\_        ________/\\\/______
 *      __/\\\\\\\\\\\\\\\\_        _____\/\\\_        ______/\\\/________
 *       _\///////////\\\//__        _____\/\\\_        ____/\\\/__________
 *        ___________\/\\\____        _____\/\\\_        __/\\\/____________
 *         ___________\///_____        _____\///_         _\///______________
 * ===========================================================================
 *  Author   : James YinG
 *  Email    : james@taogame.com
 *  业务：软件开发 | 功能定制 | Bug修复 | 项目部署
 *  专业接单，品质保障，欢迎合作！
 * ===========================================================================
 */

using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Serilog;
using ZJAdmin.Api.Data;
using ZJAdmin.Api.Middleware;
using ZJAdmin.Api.Services;

// DatabaseProvider extension
static string GetDatabaseProvider(WebApplicationBuilder builder)
{
    return builder.Configuration.GetValue<string>("DatabaseProvider") ?? "Sqlite";
}

static bool IsMySql(string provider) =>
    "MySql".Equals(provider, StringComparison.OrdinalIgnoreCase);

static void ConfigureDbContext(DbContextOptionsBuilder options, string provider, string connStr)
{
    if (IsMySql(provider))
        options.UseMySql(connStr, ServerVersion.AutoDetect(connStr));
    else
        options.UseSqlite(connStr);
}

var builder = WebApplication.CreateBuilder(args);

// Startup banner
var esc = "\x1b";
Console.WriteLine();
Console.WriteLine($@"{esc}[38;2;41;105;225m{esc}[1m____________/\\\____                ______/\\\_                __/\\\\\\\\\\\\\\\_        {esc}[0m");
Console.WriteLine($@"{esc}[38;2;52;122;235m{esc}[1m __________/\\\\\____                __/\\\\\\\_                _\/////////////\\\_       {esc}[0m");
Console.WriteLine($@"{esc}[38;2;70;140;240m{esc}[1m  ________/\\\/\\\____                _\/////\\\_                ____________/\\\/__      {esc}[0m");
Console.WriteLine($@"{esc}[38;2;100;110;235m{esc}[1m   ______/\\\/\/\\\____                _____\/\\\_                __________/\\\/____     {esc}[0m");
Console.WriteLine($@"{esc}[38;2;145;82;215m{esc}[1m    ____/\\\/__\/\\\____                _____\/\\\_                ________/\\\/______    {esc}[0m");
Console.WriteLine($@"{esc}[38;2;185;60;180m{esc}[1m     __/\\\\\\\\\\\\\\\\_                _____\/\\\_                ______/\\\/________   {esc}[0m");
Console.WriteLine($@"{esc}[38;2;215;50;140m{esc}[1m      _\///////////\\\//__                _____\/\\\_                ____/\\\/__________  {esc}[0m");
Console.WriteLine($@"{esc}[38;2;238;58;90m{esc}[1m       ___________\/\\\____                _____\/\\\_                __/\\\/____________ {esc}[0m");
Console.WriteLine($@"{esc}[38;2;245;105;50m{esc}[1m        ___________\///_____                _____\///_                 _\///______________{esc}[0m");
Console.WriteLine();
Console.WriteLine($@"  {esc}[38;2;130;140;150m{esc}[1m╔══════════════════════════════════════════════════════════════════════════╗{esc}[0m");
Console.WriteLine($@"  {esc}[38;2;130;140;150m{esc}[1m║{esc}[0m  {esc}[38;2;80;220;140m{esc}[1mAuthor   : James YinG{esc}[0m  {esc}[38;2;80;180;255m{esc}[1mEmail : james@taogame.com{esc}[0m  {esc}[38;2;255;200;60m{esc}[0m  {esc}[38;2;130;140;150m{esc}[1m                    ║{esc}[0m");
Console.WriteLine($@"  {esc}[38;2;130;140;150m{esc}[1m║{esc}[0m  {esc}[38;2;80;220;140m{esc}[1mGitHub   : github.com/JcSoftEar/zjadmin{esc}[0m{esc}[38;2;130;140;150m{esc}[1m                                    ║{esc}[0m");
Console.WriteLine($@"  {esc}[38;2;130;140;150m{esc}[1m║{esc}[0m  {esc}[38;2;255;200;60m{esc}[1m业务：软件开发 | 定制 | 修复 | 部署专业接单，品质保障，欢迎合作！{esc}[0m{esc}[38;2;130;140;150m{esc}[1m      ║{esc}[0m");
Console.WriteLine($@"  {esc}[38;2;130;140;150m{esc}[1m╚══════════════════════════════════════════════════════════════════════════╝{esc}[0m");
Console.WriteLine();

// Serilog
Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .Enrich.FromLogContext()
    .WriteTo.Console(
        outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss} [{Level:u3}] {SourceContext}{NewLine}  {Message:lj}{NewLine}{Exception}"
    )
    .WriteTo.File("logs/log-.txt", rollingInterval: RollingInterval.Day)
    .CreateLogger();

builder.Host.UseSerilog();

// Database
var dbProvider = GetDatabaseProvider(builder);
var dbConnStr = IsMySql(dbProvider)
    ? builder.Configuration.GetConnectionString("MySqlConnection")!
    : builder.Configuration.GetConnectionString("DefaultConnection")!;

Log.Information("Database Provider: {Provider}", dbProvider);

builder.Services.AddDbContext<AppDbContext>(options =>
    ConfigureDbContext(options, dbProvider, dbConnStr));

// JWT Authentication
var jwtSettings = builder.Configuration.GetSection("Jwt");
var jwtKey = Encoding.UTF8.GetBytes(jwtSettings["Key"]!);

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = jwtSettings["Issuer"],
        ValidAudience = jwtSettings["Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(jwtKey),
        ClockSkew = TimeSpan.Zero
    };
});

builder.Services.AddAuthorization();

// Services
builder.Services.AddScoped<AuthService>();
builder.Services.AddScoped<UserService>();
builder.Services.AddScoped<RoleService>();
builder.Services.AddScoped<PermissionService>();
builder.Services.AddScoped<LogService>();
builder.Services.AddScoped<ConfigService>();

// Permission filter as scoped (since it depends on AuthService)
builder.Services.AddScoped<PermissionAuthorizationFilter>();

// Controllers
builder.Services.AddControllers(options =>
{
    options.Filters.Add<PermissionAuthorizationFilter>();
});
builder.Services.AddEndpointsApiExplorer();

// Swagger
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "ZJAdmin API", Version = "v1" });
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using the Bearer scheme",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });
    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});

// CORS
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

var app = builder.Build();

// Migrate database
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    if (IsMySql(dbProvider))
        db.Database.EnsureCreated();
    else
        db.Database.Migrate();
}

app.UseSerilogRequestLogging();
app.UseCors();
app.UseAuthentication();
app.UseAuthorization();
app.UseMiddleware<ExceptionMiddleware>();
app.UseMiddleware<OperationLogMiddleware>();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapControllers();

app.Run();
