using Microsoft.Extensions.Logging;
using Wardrobe.Data.Entities;
using Wardrobe.Repositories.Interfaces;
using Wardrobe.Services.Interfaces;

namespace Wardrobe.Services.Implementations;

public class RoleService : IRoleService
{
	private readonly IRoleRepository _repository;

	private readonly ILogger<RoleService> _logger;


	public RoleService(
		IRoleRepository repository,
		ILogger<RoleService> logger)
	{
		_repository = repository;

		_logger = logger;
	}


	public async Task<Role?> GetByIdAsync(int id)
	{
		_logger.LogInformation(
			"Getting role by id {RoleId}",
			id);

		return await _repository.GetByIdAsync(id);
	}


	public async Task<Role?> GetByNameAsync(string name)
	{
		_logger.LogInformation(
			"Getting role by name {RoleName}",
			name);

		return await _repository.GetByNameAsync(name);
	}


	public async Task<IEnumerable<Role>> GetAllAsync()
	{
		_logger.LogInformation(
			"Getting all roles");

		return await _repository.GetAllAsync();
	}
}