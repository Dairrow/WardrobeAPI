using Microsoft.AspNetCore.Mvc;
using Wardrobe.API.DTOs.Auth;
using Wardrobe.Services.Interfaces;

namespace Wardrobe.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _service;


    public AuthController(
        IAuthService service)
    {
        _service = service;
    }


    [HttpPost("register")]
    public async Task<IActionResult> Register(
        RegisterDto dto)
    {
        await _service.RegisterAsync(
            dto.Username,
            dto.Email,
            dto.Password);

        return Ok();
    }


    [HttpPost("login")]
    public async Task<ActionResult<AuthResponseDto>>
        Login(
            LoginDto dto)
    {
        var token =
            await _service.LoginAsync(
                dto.Email,
                dto.Password);


        return Ok(
            new AuthResponseDto
            {
                Token = token
            });
    }
}