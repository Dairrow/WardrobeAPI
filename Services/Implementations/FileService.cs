using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Wardrobe.Services.Exceptions;
using Wardrobe.Services.Interfaces;

namespace Wardrobe.Services.Implementations;

public class FileService
    : IFileService
{
    private readonly IWebHostEnvironment
        _environment;


    private readonly string[]
        _allowedExtensions =
        [
            ".jpg",
            ".jpeg",
            ".png",
            ".webp"
        ];


    private const long MaxFileSize =
        5 * 1024 * 1024;


    public FileService(
        IWebHostEnvironment environment)
    {
        _environment = environment;
    }


    public async Task<string>
        UploadImageAsync(
            IFormFile file)
    {
        if (file.Length == 0)
        {
            throw new ValidationException(
                "File is empty");
        }


        if (file.Length > MaxFileSize)
        {
            throw new ValidationException(
                "Max file size is 5 MB");
        }


        var extension =
            Path.GetExtension(
                file.FileName)
            .ToLower();


        if (!_allowedExtensions
            .Contains(extension))
        {
            throw new ValidationException(
                "Invalid file type");
        }


        var uploadsPath =
            Path.Combine(
                _environment.WebRootPath,
                "uploads");


        if (!Directory.Exists(
                uploadsPath))
        {
            Directory.CreateDirectory(
                uploadsPath);
        }


        var fileName =
            $"{Guid.NewGuid()}" +
            extension;


        var fullPath =
            Path.Combine(
                uploadsPath,
                fileName);


        await using var stream =
            new FileStream(
                fullPath,
                FileMode.Create);


        await file.CopyToAsync(
            stream);


        return
            $"uploads/{fileName}";
    }


    public Task DeleteFileAsync(
        string filePath)
    {
        var fullPath =
            Path.Combine(
                _environment.WebRootPath,
                filePath);


        if (File.Exists(
                fullPath))
        {
            File.Delete(
                fullPath);
        }


        return Task.CompletedTask;
    }
}