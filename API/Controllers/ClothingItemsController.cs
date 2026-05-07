using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
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
        var userId = GetCurrentUserId();

        var items = await _service.GetByUserIdAsync(userId);

        return Ok(_mapper.Map<IEnumerable<ClothingItemDto>>(items));
    }

    [HttpPost]
    public async Task<IActionResult> Create(CreateClothingItemDto dto)
    {
        var entity = _mapper.Map<ClothingItem>(dto);

        entity.UserId = GetCurrentUserId();

        var created = await _service.CreateAsync(entity);

        return Ok(_mapper.Map<ClothingItemDto>(created));
    }

    private int GetCurrentUserId()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out int userId))
        {
            throw new UnauthorizedAccessException("User ID not found in token");
        }

        return userId;
    }
}