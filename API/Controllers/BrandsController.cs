using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Wardrobe.API.DTOs.Brands;
using Wardrobe.Data.Entities;
using Wardrobe.Services.Interfaces;

namespace Wardrobe.API.Controllers;

[ApiController]
[Authorize]
[Route("api/[controller]")]
public class BrandsController : ControllerBase
{
    private readonly IBrandService _service;

    private readonly IMapper _mapper;


    public BrandsController(
        IBrandService service,
        IMapper mapper)
    {
        _service = service;

        _mapper = mapper;
    }


    [HttpGet]
    public async Task<ActionResult<IEnumerable<BrandDto>>> GetAll()
    {
        var brands =
            await _service.GetAllAsync();


        return Ok(
            _mapper.Map<IEnumerable<BrandDto>>(
                brands));
    }


    [HttpGet("{id:int}")]
    public async Task<ActionResult<BrandDto>> GetById(
        int id)
    {
        var brand =
            await _service.GetByIdAsync(id);


        if (brand is null)
        {
            return NotFound();
        }


        return Ok(
            _mapper.Map<BrandDto>(
                brand));
    }


    [HttpPost]
    public async Task<ActionResult<BrandDto>> Create(
        CreateBrandDto dto)
    {
        var entity =
            _mapper.Map<Brand>(
                dto);


        var created =
            await _service.CreateAsync(
                entity);


        var result =
            _mapper.Map<BrandDto>(
                created);


        return CreatedAtAction(
            nameof(GetById),
            new { id = result.Id },
            result);
    }

    [HttpPut("{id:int}")]
    public async Task<ActionResult<
    BrandDto>> Update(
        int id,
        UpdateBrandDto dto)
    {
        var entity =
            _mapper.Map<Brand>(
                dto);


        var updated =
            await _service.UpdateAsync(
                id,
                entity);


        return Ok(
            _mapper.Map<BrandDto>(
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