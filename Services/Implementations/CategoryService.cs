using Microsoft.Extensions.Logging;
using Wardrobe.Data.Entities;
using Wardrobe.Repositories.Interfaces;
using Wardrobe.Services.Interfaces;

namespace Wardrobe.Services.Implementations;

public class CategoryService : ICategoryService
{
    private readonly ICategoryRepository _repository;

    private readonly ILogger<CategoryService> _logger;


    public CategoryService(
        ICategoryRepository repository,
        ILogger<CategoryService> logger)
    {
        _repository = repository;

        _logger = logger;
    }


    public async Task<IEnumerable<Category>> GetAllAsync()
    {
        _logger.LogInformation(
            "Getting categories");

        return await _repository.GetAllAsync();
    }


    public async Task<Category?> GetByIdAsync(int id)
    {
        return await _repository.GetByIdAsync(
            id);
    }


    public async Task<Category> CreateAsync(
        Category category)
    {
        _logger.LogInformation(
            "Creating category {Name}",
            category.Name);

        return await _repository.AddAsync(
            category);
    }
}