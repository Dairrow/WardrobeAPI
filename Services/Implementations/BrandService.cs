using Microsoft.Extensions.Logging;
using Wardrobe.Data.Entities;
using Wardrobe.Repositories.Interfaces;
using Wardrobe.Services.Exceptions;
using Wardrobe.Services.Interfaces;

namespace Wardrobe.Services.Implementations;

public class BrandService : IBrandService
{
	private readonly IBrandRepository _repository;

	private readonly ILogger<BrandService> _logger;


	public BrandService(
		IBrandRepository repository,
		ILogger<BrandService> logger)
	{
		_repository = repository;

		_logger = logger;
	}


	public async Task<IEnumerable<Brand>> GetAllAsync()
	{
		return await _repository.GetAllAsync();
	}


	public async Task<Brand?> GetByIdAsync(int id)
	{
		return await _repository.GetByIdAsync(
			id);
	}


	public async Task<Brand> CreateAsync(
		Brand brand)
	{
		_logger.LogInformation(
			"Creating brand {Name}",
			brand.Name);

		return await _repository.AddAsync(
			brand);
	}

	public async Task<Brand>
	UpdateAsync(
		int id,
		Brand brand)
	{
		var existing =
			await _repository
				.GetByIdAsync(id);


		if (existing is null)
		{
			throw new NotFoundException(
				"Brand not found");
		}


		existing.Name =
			brand.Name;


		await _repository
			.UpdateAsync(
				existing);


		return existing;
	}

	public async Task DeleteAsync(
		int id)
	{
		var existing =
			await _repository
				.GetByIdAsync(id);


		if (existing is null)
		{
			throw new NotFoundException(
				"Brand not found");
		}


		await _repository
			.DeleteAsync(
				existing);
	}
}