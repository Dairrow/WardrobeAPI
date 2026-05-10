using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Wardrobe.API.DTOs.Categories;
using Wardrobe.Data.Entities;
using Wardrobe.Services.Interfaces;

namespace Wardrobe.API.Controllers;

[ApiController]
[Authorize]
[Route("api/[controller]")]
public class CategoriesController : ControllerBase
{
	private readonly ICategoryService _service;

	private readonly IMapper _mapper;


	public CategoriesController(
		ICategoryService service,
		IMapper mapper)
	{
		_service = service;

		_mapper = mapper;
	}


	[HttpGet]
	[ProducesResponseType(200)]
	public async Task<ActionResult<IEnumerable<CategoryDto>>> GetAll()
	{
		var categories =
			await _service.GetAllAsync();


		var result =
			_mapper.Map<IEnumerable<CategoryDto>>(
				categories);


		return Ok(result);
	}


	[HttpGet("{id:int}")]
	[ProducesResponseType(200)]
	[ProducesResponseType(404)]
	public async Task<ActionResult<CategoryDto>> GetById(
		int id)
	{
		var category =
			await _service.GetByIdAsync(id);


		if (category is null)
		{
			return NotFound();
		}


		var result =
			_mapper.Map<CategoryDto>(
				category);


		return Ok(result);
	}


	[HttpPost]
	[Authorize(Roles = "Admin")]
	[ProducesResponseType(201)]
	[ProducesResponseType(400)]
	public async Task<ActionResult<CategoryDto>> Create(
		CreateCategoryDto dto)
	{
		var category =
			_mapper.Map<Category>(
				dto);


		var created =
			await _service.CreateAsync(
				category);


		var result =
			_mapper.Map<CategoryDto>(
				created);


		return CreatedAtAction(
			nameof(GetById),
			new { id = result.Id },
			result);
	}

	[HttpPut("{id:int}")]
	[Authorize(Roles = "Admin")]
	[ProducesResponseType(200)]
	[ProducesResponseType(404)]
	public async Task<ActionResult<
	CategoryDto>> Update(
		int id,
		UpdateCategoryDto dto)
	{
		var entity =
			_mapper.Map<Category>(
				dto);


		var updated =
			await _service.UpdateAsync(
				id,
				entity);


		return Ok(
			_mapper.Map<CategoryDto>(
				updated));
	}


	[HttpDelete("{id:int}")]
	[Authorize(Roles = "Admin")]
	[ProducesResponseType(204)]
	[ProducesResponseType(404)]
	public async Task<IActionResult>
		Delete(
			int id)
	{
		await _service.DeleteAsync(
			id);


		return NoContent();
	}
}