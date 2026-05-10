using Microsoft.AspNetCore.Http;

namespace Wardrobe.Services.Interfaces;

public interface IFileService
{
	Task<string> UploadImageAsync(
		IFormFile file);


	Task DeleteFileAsync(
		string filePath);
}