using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Wardrobe.API.DTOs.Files;
using Wardrobe.Services.Interfaces;

namespace Wardrobe.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Admin")]
public class FilesController
	: ControllerBase
{
	private readonly IFileService
		_fileService;


	public FilesController(
		IFileService fileService)
	{
		_fileService = fileService;
	}


	[HttpPost("upload")]
	[Authorize(Roles = "Admin")]
	[ProducesResponseType(200)]
	[ProducesResponseType(400)]
	public async Task<ActionResult<
	FileUploadResponseDto>>
	Upload(
		[FromForm]
		IFormFile file)
	{
		var path =
			await _fileService
				.UploadImageAsync(
					file);


		return Ok(
			new FileUploadResponseDto
			{
				Path = path
			});
	}


	[HttpDelete]
	[Authorize(Roles = "Admin")]
	[ProducesResponseType(204)]
	public async Task<IActionResult>
		Delete(
			string path)
	{
		await _fileService
			.DeleteFileAsync(
				path);


		return NoContent();
	}
}