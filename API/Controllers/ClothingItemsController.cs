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
    private readonly IFileService _fileService;
    private readonly IMapper _mapper;


    public ClothingItemsController(
        IClothingItemService service,
        IFileService fileService,
        IMapper mapper)
    {
        _service = service;

        _fileService = fileService;

        _mapper = mapper;
    }


    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var userId = GetCurrentUserId();

        var items = await _service.GetByUserIdAsync(userId);

        return Ok(_mapper.Map<IEnumerable<ClothingItemDto>>(items));
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<ClothingItemDto>> GetById(int id)
    {
        var userId = GetCurrentUserId();
        var item = await _service.GetByIdAsync(id, userId);

        if (item is null)
            return NotFound();

        return Ok(_mapper.Map<ClothingItemDto>(item));
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(201)]
    public async Task<ActionResult<
        ClothingItemDto>>
        Create(
            [FromForm]
        CreateClothingItemDto dto)
    {
        var entity =
            _mapper.Map<
                ClothingItem>(
                    dto);

        entity.UserId = GetCurrentUserId();


        if (dto.Image is not null)
        {
            entity.ImagePath =
                await _fileService
                    .UploadImageAsync(
                        dto.Image);
        }


        var created =
            await _service
                .CreateAsync(
                    entity);


        var result =
            _mapper.Map<
                ClothingItemDto>(
                    created);


        return CreatedAtAction(
            nameof(GetById),
            new { id = result.Id },
            result);
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

    [HttpPut("{id:int}")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<ClothingItemDto>> Update(int id, [FromForm] UpdateClothingItemDto dto)
    {
        var userId = GetCurrentUserId();
        var existing = await _service.GetByIdAsync(id, userId);

        if (existing is null)
            return NotFound("Clothing item not found");

        if (dto.Image is not null && !string.IsNullOrWhiteSpace(existing.ImagePath))
        {
            await _fileService.DeleteFileAsync(existing.ImagePath);
        }

        var entity = _mapper.Map<ClothingItem>(dto);

        if (dto.Image is not null)
        {
            entity.ImagePath = await _fileService.UploadImageAsync(dto.Image);
        }

        var updated = await _service.UpdateAsync(id, entity);

        return Ok(_mapper.Map<ClothingItemDto>(updated));
    }

    [HttpDelete("{id:int}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Delete(int id)
    {
        var userId = GetCurrentUserId();
        var existing = await _service.GetByIdAsync(id, userId);

        if (existing is null)
            return NotFound("Clothing item not found");

        if (!string.IsNullOrWhiteSpace(existing.ImagePath))
        {
            await _fileService.DeleteFileAsync(existing.ImagePath);
        }

        await _service.DeleteAsync(id);

        return NoContent();
    }
}