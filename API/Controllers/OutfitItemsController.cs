using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Wardrobe.API.DTOs.OutfitItems;
using Wardrobe.API.DTOs.ClothingItems;
using Wardrobe.Services.Interfaces;

namespace Wardrobe.API.Controllers;

[ApiController]
[Authorize]
[Route("api/[controller]")]
public class OutfitItemsController : ControllerBase
{
    private readonly IOutfitItemService _outfitItemService;
    private readonly IOutfitService _outfitService;
    private readonly IMapper _mapper;

    public OutfitItemsController(
        IOutfitItemService outfitItemService,
        IOutfitService outfitService,
        IMapper mapper)
    {
        _outfitItemService = outfitItemService;
        _outfitService = outfitService;
        _mapper = mapper;
    }

    [HttpGet("outfit/{outfitId:int}")]
    public async Task<IActionResult> GetByOutfit(int outfitId)
    {
        try
        {
            var userId = GetCurrentUserId();
            var items = await _outfitItemService.GetByOutfitIdAsync(outfitId, userId);

            return Ok(_mapper.Map<IEnumerable<OutfitItemDto>>(items));
        }
        catch (UnauthorizedAccessException ex)
        {
            return NotFound(ex.Message);
        }
    }

    [HttpGet("outfit/{outfitId:int}/details")]
    public async Task<IActionResult> GetOutfitDetails(int outfitId)
    {
        var userId = GetCurrentUserId();
        var outfit = await _outfitService.GetByIdAsync(outfitId, userId);

        if (outfit == null)
        {
            return NotFound("Outfit not found");
        }

        var items = await _outfitItemService.GetByOutfitIdAsync(outfitId, userId);

        var result = new OutfitDetailDto
        {
            Id = outfit.Id,
            Name = outfit.Name,
            Items = _mapper.Map<List<ClothingItemDto>>(items.Select(x => x.ClothingItem))
        };

        return Ok(result);
    }

    [HttpPost]
    public async Task<IActionResult> AddItemToOutfit(CreateOutfitItemDto dto)
    {
        try
        {
            var userId = GetCurrentUserId();

            var outfitItem = await _outfitItemService.AddAsync(
                dto.OutfitId,
                dto.ClothingItemId,
                userId);

            var items = await _outfitItemService.GetByOutfitIdAsync(dto.OutfitId, userId);
            var createdItem = items.FirstOrDefault(x =>
                x.OutfitId == dto.OutfitId &&
                x.ClothingItemId == dto.ClothingItemId);

            return Ok(_mapper.Map<OutfitItemDto>(createdItem));
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(ex.Message);
        }
    }

    [HttpDelete]
    public async Task<IActionResult> RemoveItemFromOutfit([FromBody] CreateOutfitItemDto dto)
    {
        try
        {
            var userId = GetCurrentUserId();
            await _outfitItemService.DeleteAsync(dto.OutfitId, dto.ClothingItemId, userId);
            return NoContent();
        }
        catch (ArgumentException ex)
        {
            return NotFound(ex.Message);
        }
    }

    protected int GetCurrentUserId()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out int userId))
        {
            throw new UnauthorizedAccessException("User ID not found in token");
        }

        return userId;
    }
}