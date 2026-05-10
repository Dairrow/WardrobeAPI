using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Wardrobe.API.DTOs.Roles;
using Wardrobe.Services.Interfaces;

namespace Wardrobe.API.Controllers;

[ApiController]
[Authorize(Roles = "Admin")]
[Route("api/[controller]")]
public class RolesController : ControllerBase
{
	private readonly IRoleService _service;

	private readonly IMapper _mapper;


	public RolesController(
		IRoleService service,
		IMapper mapper)
	{
		_service = service;

		_mapper = mapper;
	}


	[HttpGet]
	public async Task<ActionResult<IEnumerable<RoleDto>>> GetAll()
	{
		var roles =
			await _service.GetAllAsync();


		return Ok(
			_mapper.Map<IEnumerable<RoleDto>>(
				roles));
	}


	[HttpGet("{id:int}")]
	public async Task<ActionResult<RoleDto>> GetById(
		int id)
	{
		var role =
			await _service.GetByIdAsync(id);


		if (role is null)
		{
			return NotFound();
		}


		return Ok(
			_mapper.Map<RoleDto>(
				role));
	}
}