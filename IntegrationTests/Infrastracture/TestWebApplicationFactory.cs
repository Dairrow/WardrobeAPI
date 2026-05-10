using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;

namespace IntegrationTests.Infrastructure;

 class TestWebApplicationFactory : WebApplicationFactory<Program>, IAsyncLifetime
{
	private readonly string _connectionString;

	public TestWebApplicationFactory(string connectionString)
	{
		_connectionString = connectionString;
	}

	protected override void ConfigureWebHost(IWebHostBuilder builder)
	{
		builder.UseEnvironment("Testing");

		builder.ConfigureAppConfiguration((context, config) =>
		{
			var testSettings = new Dictionary<string, string?>
			{
				["ConnectionStrings:DefaultConnection"] = _connectionString
			};

			config.AddInMemoryCollection(testSettings);
		});
	}

	public async Task InitializeAsync()
	{
		using var scope = Services.CreateScope();
		var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

		await context.Database.MigrateAsync();
	}

	public new async Task DisposeAsync()
	{
		await base.DisposeAsync();
	}
}