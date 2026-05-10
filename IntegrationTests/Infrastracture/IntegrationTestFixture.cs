using Testcontainers.PostgreSql;

namespace IntegrationTests.Infrastructure;

public class IntegrationTestFixture : IAsyncLifetime
{
	private readonly PostgreSqlContainer _container;

	public string ConnectionString { get; private set; } = string.Empty;

	public IntegrationTestFixture()
	{
		_container = new PostgreSqlBuilder()
			.WithDatabase("wardrobe_test")
			.WithUsername("test_user")
			.WithPassword("test_password_123")
			.WithImage("postgres:16-alpine")
			.WithCleanUp(true)
			.Build();
	}

	public async Task InitializeAsync()
	{
		await _container.StartAsync();
		ConnectionString = _container.GetConnectionString();
	}

	public async Task DisposeAsync()
	{
		await _container.StopAsync();
		await _container.DisposeAsync();
	}

	internal async Task<TestWebApplicationFactory> CreateFactoryAsync()
	{
		var factory = new TestWebApplicationFactory(ConnectionString);
		await factory.InitializeAsync();
		return factory;
	}
}