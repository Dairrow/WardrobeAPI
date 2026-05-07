using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Wardrobe.API.DTOs.Outfits;
using Wardrobe.Data.Entities;
using Wardrobe.Services.Interfaces;

namespace Wardrobe.API.Controllers;

[ApiController]
[Authorize]
[Route("api/[controller]")]
public class OutfitsController 
    : ControllerBase
{
    private readonly IOutfitService _service;

    private readonly IMapper _mapper;


    public OutfitsController(
        IOutfitService service,
        IMapper mapper)
    {
        _service = service;

        _mapper = mapper;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var userId = GetCurrentUserId();

        var outfits = await _service.GetByUserIdAsync(userId);

        return Ok(_mapper.Map<IEnumerable<OutfitDto>>(outfits));
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id)
    {
        var userId = GetCurrentUserId();

        var outfit = await _service.GetByIdAsync(id, userId);

        if (outfit is null)
        {
            return NotFound();
        }

        return Ok(_mapper.Map<OutfitDto>(outfit));
    }

    [HttpPost]
    public async Task<IActionResult> Create(CreateOutfitDto dto)
    {
        var entity = _mapper.Map<Outfit>(dto);

        entity.UserId = GetCurrentUserId();

        var created = await _service.CreateAsync(entity);

        return CreatedAtAction(
            nameof(GetById),
            new { id = created.Id },
            _mapper.Map<OutfitDto>(created));
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