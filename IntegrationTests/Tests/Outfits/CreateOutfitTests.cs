using Wardrobe.API.DTOs.Outfits;

namespace IntegrationTests.Tests.Outfits;

[Collection("IntegrationTests")]
public class CreateOutfitTests
{
    private readonly IntegrationTestFixture _fixture;

    public CreateOutfitTests(IntegrationTestFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact]
    public async Task Create_WithValidDataAndToken_ReturnsCreated()
    {
        await using var factory = await _fixture.CreateFactoryAsync();
        var client = factory.CreateClient();

        using var scope = factory.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        await TestDataSeeder.SeedRolesAsync(context);
        var user = await TestDataSeeder.SeedUserAsync(
            context,
            $"User_{Guid.NewGuid().ToString("N")[..6]}",
            $"u_{Guid.NewGuid().ToString("N")[..6]}@test.com",
            "Pass123!");

        var outfitName = $"New Outfit {Guid.NewGuid().ToString("N")[..6]}";

        var token = JwtTokenGenerator.GenerateToken(user.Id, user.Email, "User");
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var dto = new CreateOutfitDto { Name = outfitName };

        var response = await client.PostAsJsonAsync("/api/outfits", dto);

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);

        var outfitDto = await response.Content.ReadFromJsonAsync<OutfitDto>();
        Assert.NotNull(outfitDto);
        Assert.NotEqual(0, outfitDto.Id);
        Assert.Equal(outfitName, outfitDto.Name);
    }

    [Fact]
    public async Task Create_WithoutToken_ReturnsUnauthorized()
    {
        await using var factory = await _fixture.CreateFactoryAsync();
        var client = factory.CreateClient();

        var dto = new CreateOutfitDto { Name = "Test Outfit" };

        var response = await client.PostAsJsonAsync("/api/outfits", dto);

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task Create_WithEmptyName_ReturnsBadRequest()
    {
        await using var factory = await _fixture.CreateFactoryAsync();
        var client = factory.CreateClient();

        using var scope = factory.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        await TestDataSeeder.SeedRolesAsync(context);
        var user = await TestDataSeeder.SeedUserAsync(
            context,
            $"User_{Guid.NewGuid().ToString("N")[..6]}",
            $"em_{Guid.NewGuid().ToString("N")[..6]}@test.com",
            "Pass123!");

        var token = JwtTokenGenerator.GenerateToken(user.Id, user.Email, "User");
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var dto = new CreateOutfitDto { Name = "" };

        var response = await client.PostAsJsonAsync("/api/outfits", dto);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task Create_WithTooLongName_ReturnsInternalServerError()
    {
        await using var factory = await _fixture.CreateFactoryAsync();
        var client = factory.CreateClient();

        using var scope = factory.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        await TestDataSeeder.SeedRolesAsync(context);
        var user = await TestDataSeeder.SeedUserAsync(
            context,
            $"User_{Guid.NewGuid().ToString("N")[..6]}",
            $"long_{Guid.NewGuid().ToString("N")[..6]}@test.com",
            "Pass123!");

        var token = JwtTokenGenerator.GenerateToken(user.Id, user.Email, "User");
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var dto = new CreateOutfitDto { Name = new string('X', 150) };

        var response = await client.PostAsJsonAsync("/api/outfits", dto);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task Create_SetsUserIdFromTokenAutomatically()
    {
        await using var factory = await _fixture.CreateFactoryAsync();
        var client = factory.CreateClient();

        using var scope = factory.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        await TestDataSeeder.SeedRolesAsync(context);
        var user = await TestDataSeeder.SeedUserAsync(
            context,
            $"TokenUser_{Guid.NewGuid().ToString("N")[..6]}",
            $"token_{Guid.NewGuid().ToString("N")[..6]}@test.com",
            "Pass123!");

        var outfitName = $"Auto UserId {Guid.NewGuid().ToString("N")[..6]}";

        var token = JwtTokenGenerator.GenerateToken(user.Id, user.Email, "User");
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var dto = new CreateOutfitDto { Name = outfitName };

        var response = await client.PostAsJsonAsync("/api/outfits", dto);

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);

        var outfitDto = await response.Content.ReadFromJsonAsync<OutfitDto>();
        var savedOutfit = await context.Outfits
            .AsNoTracking()
            .FirstOrDefaultAsync(o => o.Id == outfitDto!.Id);

        Assert.NotNull(savedOutfit);
        Assert.Equal(user.Id, savedOutfit.UserId);
    }
}