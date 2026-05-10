using Wardrobe.API.DTOs.OutfitItems;

namespace IntegrationTests.Tests.OutfitItems;

[Collection("IntegrationTests")]
public class GetOutfitItemsTests
{
    private readonly IntegrationTestFixture _fixture;

    public GetOutfitItemsTests(IntegrationTestFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact]
    public async Task GetByOutfit_WithValidToken_ReturnsItems()
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

        var category = await TestDataSeeder.SeedCategoryAsync(
            context,
            $"Cat_{Guid.NewGuid().ToString("N")[..6]}");

        var brand = await TestDataSeeder.SeedBrandAsync(
            context,
            $"Brand_{Guid.NewGuid().ToString("N")[..6]}");

        var outfit = await TestDataSeeder.SeedOutfitAsync(
            context,
            user.Id,
            $"Outfit {Guid.NewGuid().ToString("N")[..6]}");

        var item1 = await TestDataSeeder.SeedClothingItemAsync(
            context, user.Id, category.Id, brand.Id, $"Item 1 {Guid.NewGuid().ToString("N")[..6]}", 10.00m);

        var item2 = await TestDataSeeder.SeedClothingItemAsync(
            context, user.Id, category.Id, brand.Id, $"Item 2 {Guid.NewGuid().ToString("N")[..6]}", 20.00m);

        await TestDataSeeder.SeedOutfitItemAsync(context, outfit.Id, item1.Id);
        await TestDataSeeder.SeedOutfitItemAsync(context, outfit.Id, item2.Id);

        var token = JwtTokenGenerator.GenerateToken(user.Id, user.Email, "User");
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var response = await client.GetAsync($"/api/outfititems/outfit/{outfit.Id}");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var items = await response.Content.ReadFromJsonAsync<List<OutfitItemDto>>();
        Assert.NotNull(items);
        Assert.Equal(2, items.Count);
    }

    [Fact]
    public async Task GetByOutfit_WithOtherUserToken_ReturnsNotFound()
    {
        await using var factory = await _fixture.CreateFactoryAsync();
        var client = factory.CreateClient();

        using var scope = factory.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        await TestDataSeeder.SeedRolesAsync(context);

        var owner = await TestDataSeeder.SeedUserAsync(
            context,
            $"Owner_{Guid.NewGuid().ToString("N")[..6]}",
            $"owner_{Guid.NewGuid().ToString("N")[..6]}@test.com",
            "Pass123!");

        var otherUser = await TestDataSeeder.SeedUserAsync(
            context,
            $"Other_{Guid.NewGuid().ToString("N")[..6]}",
            $"other_{Guid.NewGuid().ToString("N")[..6]}@test.com",
            "Pass123!");

        var outfit = await TestDataSeeder.SeedOutfitAsync(
            context,
            owner.Id,
            $"Outfit {Guid.NewGuid().ToString("N")[..6]}");

        var token = JwtTokenGenerator.GenerateToken(otherUser.Id, otherUser.Email, "User");
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var response = await client.GetAsync($"/api/outfititems/outfit/{outfit.Id}");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task GetByOutfit_WithoutToken_ReturnsUnauthorized()
    {
        await using var factory = await _fixture.CreateFactoryAsync();
        var client = factory.CreateClient();

        var response = await client.GetAsync("/api/outfititems/outfit/1");

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task GetOutfitDetails_WithValidToken_ReturnsOutfitWithItems()
    {
        await using var factory = await _fixture.CreateFactoryAsync();
        var client = factory.CreateClient();

        using var scope = factory.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        await TestDataSeeder.SeedRolesAsync(context);
        var user = await TestDataSeeder.SeedUserAsync(
            context,
            $"User_{Guid.NewGuid().ToString("N")[..6]}",
            $"det_{Guid.NewGuid().ToString("N")[..6]}@test.com",
            "Pass123!");

        var category = await TestDataSeeder.SeedCategoryAsync(
            context,
            $"Cat_{Guid.NewGuid().ToString("N")[..6]}");

        var brand = await TestDataSeeder.SeedBrandAsync(
            context,
            $"Brand_{Guid.NewGuid().ToString("N")[..6]}");

        var outfit = await TestDataSeeder.SeedOutfitAsync(
            context,
            user.Id,
            $"Detailed Outfit {Guid.NewGuid().ToString("N")[..6]}");

        var item = await TestDataSeeder.SeedClothingItemAsync(
            context, user.Id, category.Id, brand.Id, $"Detailed Item {Guid.NewGuid().ToString("N")[..6]}", 99.99m);

        await TestDataSeeder.SeedOutfitItemAsync(context, outfit.Id, item.Id);

        var token = JwtTokenGenerator.GenerateToken(user.Id, user.Email, "User");
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var response = await client.GetAsync($"/api/outfititems/outfit/{outfit.Id}/details");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task GetOutfitDetails_WithNonExistentOutfit_ReturnsNotFound()
    {
        await using var factory = await _fixture.CreateFactoryAsync();
        var client = factory.CreateClient();

        using var scope = factory.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        await TestDataSeeder.SeedRolesAsync(context);
        var user = await TestDataSeeder.SeedUserAsync(
            context,
            $"User_{Guid.NewGuid().ToString("N")[..6]}",
            $"nf_{Guid.NewGuid().ToString("N")[..6]}@test.com",
            "Pass123!");

        var token = JwtTokenGenerator.GenerateToken(user.Id, user.Email, "User");
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var response = await client.GetAsync("/api/outfititems/outfit/99999/details");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }
}