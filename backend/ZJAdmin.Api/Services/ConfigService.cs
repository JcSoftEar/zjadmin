using Microsoft.EntityFrameworkCore;
using ZJAdmin.Api.Data;
using ZJAdmin.Api.DTOs;
using ZJAdmin.Api.Models;

namespace ZJAdmin.Api.Services;

public class ConfigService
{
    private readonly AppDbContext _db;

    public ConfigService(AppDbContext db)
    {
        _db = db;
    }

    public async Task<ApiResponse> GetAll()
    {
        var configs = await _db.Configs.OrderBy(c => c.Id).ToListAsync();

        var data = new Dictionary<string, object>();
        foreach (var c in configs)
        {
            data[c.ConfigKey] = c.ConfigValue ?? "";
        }

        return ApiResponse.Success(data);
    }

    public async Task<ApiResponse> Update(Dictionary<string, string> settings)
    {
        foreach (var (key, value) in settings)
        {
            var config = await _db.Configs.FirstOrDefaultAsync(c => c.ConfigKey == key);
            if (config != null)
            {
                config.ConfigValue = value;
                config.UpdateTime = DateTime.UtcNow;
            }
            else
            {
                _db.Configs.Add(new Config
                {
                    ConfigKey = key,
                    ConfigValue = value,
                    CreateTime = DateTime.UtcNow
                });
            }
        }

        await _db.SaveChangesAsync();
        return ApiResponse.Success(null, "保存成功");
    }
}
