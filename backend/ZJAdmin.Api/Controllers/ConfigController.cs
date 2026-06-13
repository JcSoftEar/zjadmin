using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ZJAdmin.Api.Attributes;
using ZJAdmin.Api.DTOs;
using ZJAdmin.Api.Services;

namespace ZJAdmin.Api.Controllers;

[ApiController]
[Route("api/v1/config")]
public class ConfigController : ControllerBase
{
    private readonly ConfigService _configService;

    public ConfigController(ConfigService configService)
    {
        _configService = configService;
    }

    [HttpGet]
    [AllowAnonymous]
    public async Task<ApiResponse> GetAll()
    {
        return await _configService.GetAll();
    }

    [HttpPut]
    [Authorize]
    [Permission("system:config:edit")]
    public async Task<ApiResponse> Update([FromBody] Dictionary<string, string> settings)
    {
        return await _configService.Update(settings);
    }
}
