using Wardrobe.API.DTOs.OutfitItems;

namespace IntegrationTests.Tests.OutfitItems;

[Collection("IntegrationTests")]
public class RemoveItemFromOutfitTests
{
    private readonly IntegrationTestFixture _fixture;

    public RemoveItemFromOutfitTests(IntegrationTestFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact]
    public async Task RemoveItem_WithValidData_ReturnsNoContent()
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

        var clothingItem = await TestDataSeeder.SeedClothingItemAsync(
            context, user.Id, category.Id, brand.Id, $"Remove Me {Guid.NewGuid().ToString("N")[..6]}", 15.00m);

        await TestDataSeeder.SeedOutfitItemAsync(context, outfit.Id, clothingItem.Id);

        var token = JwtTokenGenerator.GenerateToken(user.Id, user.Email, "User");
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var dto = new CreateOutfitItemDto
        {
            OutfitId = outfit.Id,
            ClothingItemId = clothingItem.Id
        };

        var request = new HttpRequestMessage(HttpMethod.Delete, "/api/outfititems")
        {
            Content = JsonContent.Create(dto)
        };
        var response = await client.SendAsync(request);

        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);

        var exists = await context.OutfitItems
            .AnyAsync(oi => oi.OutfitId == outfit.Id && oi.ClothingItemId == clothingItem.Id);
        Assert.False(exists);
    }

    [Fact]
    public async Task RemoveItem_WithNonExistentOutfit_ReturnsNotFound()
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

        var dto = new CreateOutfitItemDto
        {
            OutfitId = 99999,
            ClothingItemId = 1
        };

        var request = new HttpRequestMessage(HttpMethod.Delete, "/api/outfititems")
        {
            Content = JsonContent.Create(dto)
        };
        var response = await client.SendAsync(request);

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task RemoveItem_WithoutToken_ReturnsUnauthorized()
    {
        await using var factory = await _fixture.CreateFactoryAsync();
        var client = factory.CreateClient();

        var dto = new CreateOutfitItemDto
        {
            OutfitId = 1,
            ClothingItemId = 1
        };

        var request = new HttpRequestMessage(HttpMethod.Delete, "/api/outfititems")
        {
            Content = JsonContent.Create(dto)
        };
        var response = await client.SendAsync(request);

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }
}