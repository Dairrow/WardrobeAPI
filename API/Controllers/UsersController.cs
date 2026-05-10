using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Wardrobe.API.DTOs.Users;
using Wardrobe.Data.Entities;
using Wardrobe.Services.Interfaces;

namespace Wardrobe.API.Controllers;

[ApiController]
[Authorize(Roles = "Admin")]
[Route("api/[controller]")]
public class UsersController : ControllerBase
{
    private readonly IUserService _service;

    private readonly IMapper _mapper;


    public UsersController(
        IUserService service,
        IMapper mapper)
    {
        _service = service;

        _mapper = mapper;
    }


    [HttpGet]
    public async Task<ActionResult<IEnumerable<UserDto>>> GetAll()
    {
        var users =
            await _service.GetAllAsync();


        return Ok(
            _mapper.Map<IEnumerable<UserDto>>(
                users));
    }


    [HttpGet("{id:int}")]
    public async Task<ActionResult<UserDto>> GetById(
        int id)
    {
        var user =
            await _service.GetByIdAsync(id);


        if (user is null)
        {
            return NotFound();
        }


        return Ok(
            _mapper.Map<UserDto>(
                user));
    }


    [HttpPost]
    public async Task<ActionResult<UserDto>> Create(
        CreateUserDto dto)
    {
        var entity =
            _mapper.Map<User>(
                dto);


        var created =
            await _service.CreateAsync(
                entity);


        var result =
            _mapper.Map<UserDto>(
                created);


        return CreatedAtAction(
            nameof(GetById),
            new { id = result.Id },
            result);
    }

    [HttpPut("{id:int}")]
    public async Task<ActionResult<
    UserDto>> Update(
        int id,
        UpdateUserDto dto)
    {
        var entity =
            _mapper.Map<User>(
                dto);


        var updated =
            await _service.UpdateAsync(
                id,
                entity);


        return Ok(
            _mapper.Map<UserDto>(
                updated));
    }


    [HttpDelete("{id:int}")]
    public async Task<IActionResult>
        Delete(
            int id)
    {
        await _service.DeleteAsync(
            id);


        return NoContent();
    }
}