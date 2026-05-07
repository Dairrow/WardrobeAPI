using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
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
        var outfits =
            await _service.GetAllAsync();


        return Ok(
            _mapper.Map<
                IEnumerable<OutfitDto>>(
                    outfits));
    }


    [HttpPost]
    public async Task<IActionResult> Create(
        CreateOutfitDto dto)
    {
        var entity =
            _mapper.Map<Outfit>(
                dto);


        var created =
            await _service.CreateAsync(
                entity);


        return Ok(
            _mapper.Map<OutfitDto>(
                created));
    }
}