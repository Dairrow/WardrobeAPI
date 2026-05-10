namespace IntegrationTests.Tests.Outfits;

[Collection("IntegrationTests")]
public class DeleteOutfitTests
{
    private readonly IntegrationTestFixture _fixture;

    public DeleteOutfitTests(IntegrationTestFixture fixture)
    {
        _fixture = fixture;
    }


    [Fact]
    public async Task Delete_WithoutToken_ReturnsUnauthorized()
    {
        await using var factory = await _fixture.CreateFactoryAsync();
        var client = factory.CreateClient();

        var response = await client.DeleteAsync("/api/outfits/1");

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task Delete_WithNonExistentId_ReturnsNotFound()
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

        var response = await client.DeleteAsync("/api/outfits/99999");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task Delete_WithOutfitItems_CascadesDeletion()
    {
        await using var factory = await _fixture.CreateFactoryAsync();
        var client = factory.CreateClient();

        using var scope = factory.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        await TestDataSeeder.SeedRolesAsync(context);
        var user = await TestDataSeeder.SeedUserAsync(
            context,
            $"User_{Guid.NewGuid().ToString("N")[..6]}",
            $"cascade_{Guid.NewGuid().ToString("N")[..6]}@test.com",
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
            $"Outfit With Items {Guid.NewGuid().ToString("N")[..6]}");

        var clothingItem = await TestDataSeeder.SeedClothingItemAsync(
            context, user.Id, category.Id, brand.Id, "Item in Outfit", 15.00m);

        await TestDataSeeder.SeedOutfitItemAsync(context, outfit.Id, clothingItem.Id);

        var token = JwtTokenGenerator.GenerateToken(user.Id, user.Email, "User");
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var response = await client.DeleteAsync($"/api/outfits/{outfit.Id}");

        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);

        var remainingItems = await context.OutfitItems
            .Where(oi => oi.OutfitId == outfit.Id)
            .ToListAsync();
        Assert.Empty(remainingItems);

        var remainingClothingItem = await context.ClothingItems.FindAsync(clothingItem.Id);
        Assert.NotNull(remainingClothingItem);
    }
}