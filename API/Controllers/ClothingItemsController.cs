using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Wardrobe.API.DTOs.ClothingItems;
using Wardrobe.Data.Entities;
using Wardrobe.Services.Interfaces;

namespace Wardrobe.API.Controllers;

[ApiController]
[Authorize]
[Route("api/[controller]")]
public class ClothingItemsController
    : ControllerBase
{
    private readonly IClothingItemService _service;

    private readonly IMapper _mapper;


    public ClothingItemsController(
        IClothingItemService service,
        IMapper mapper)
    {
        _service = service;

        _mapper = mapper;
    }


    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var items =
            await _service.GetAllAsync();


        return Ok(
            _mapper.Map<
                IEnumerable<ClothingItemDto>>(
                    items));
    }


    [HttpPost]
    public async Task<IActionResult> Create(
        CreateClothingItemDto dto)
    {
        var entity =
            _mapper.Map<ClothingItem>(
                dto);


        var created =
            await _service.CreateAsync(
                entity);


        return Ok(
            _mapper.Map<ClothingItemDto>(
                created));
    }
}