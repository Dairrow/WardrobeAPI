using Wardrobe.API.DTOs.Auth;
using Wardrobe.API.DTOs.ClothingItems;
using Wardrobe.API.DTOs.OutfitItems;

namespace IntegrationTests.Tests.Security;

[Collection("IntegrationTests")]
public class AuthorizationTests
{
    private readonly IntegrationTestFixture _fixture;

    public AuthorizationTests(IntegrationTestFixture fixture)
    {
        _fixture = fixture;
    }


    [Theory]
    [InlineData("/api/brands")]
    [InlineData("/api/brands/1")]
    [InlineData("/api/categories")]
    [InlineData("/api/categories/1")]
    [InlineData("/api/clothingitems")]
    [InlineData("/api/clothingitems/1")]
    [InlineData("/api/outfits")]
    [InlineData("/api/outfits/1")]
    [InlineData("/api/outfititems/outfit/1")]
    [InlineData("/api/outfititems/outfit/1/details")]
    [InlineData("/api/users")]
    [InlineData("/api/users/1")]
    [InlineData("/api/roles")]
    [InlineData("/api/roles/1")]
    public async Task GetEndpoints_WithoutToken_ReturnsUnauthorized(string endpoint)
    {
        await using var factory = await _fixture.CreateFactoryAsync();
        var client = factory.CreateClient();

        var response = await client.GetAsync(endpoint);

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Theory]
    [InlineData("/api/brands", "POST")]
    [InlineData("/api/categories", "POST")]
    [InlineData("/api/clothingitems", "POST")]
    [InlineData("/api/outfits", "POST")]
    [InlineData("/api/outfititems", "POST")]
    [InlineData("/api/users", "POST")]
    [InlineData("/api/brands/1", "PUT")]
    [InlineData("/api/categories/1", "PUT")]
    [InlineData("/api/clothingitems/1", "PUT")]
    [InlineData("/api/outfits/1", "PUT")]
    [InlineData("/api/users/1", "PUT")]
    [InlineData("/api/brands/1", "DELETE")]
    [InlineData("/api/categories/1", "DELETE")]
    [InlineData("/api/clothingitems/1", "DELETE")]
    [InlineData("/api/outfits/1", "DELETE")]
    [InlineData("/api/users/1", "DELETE")]
    [InlineData("/api/outfititems", "DELETE")]
    public async Task MutatingEndpoints_WithoutToken_ReturnsUnauthorized(string endpoint, string method)
    {
        await using var factory = await _fixture.CreateFactoryAsync();
        var client = factory.CreateClient();

        var request = new HttpRequestMessage(new HttpMethod(method), endpoint)
        {
            Content = method is "POST" or "PUT" or "DELETE"
                ? JsonContent.Create(new { })
                : null
        };

        if (method == "DELETE" && endpoint == "/api/outfititems")
        {
            request.Content = JsonContent.Create(new { OutfitId = 1, ClothingItemId = 1 });
        }

        var response = await client.SendAsync(request);

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }


    [Theory]
    [InlineData("/api/brands", "POST")]
    [InlineData("/api/brands/1", "PUT")]
    [InlineData("/api/brands/1", "DELETE")]
    [InlineData("/api/categories", "POST")]
    [InlineData("/api/categories/1", "PUT")]
    [InlineData("/api/categories/1", "DELETE")]
    [InlineData("/api/clothingitems", "POST")]
    [InlineData("/api/clothingitems/1", "PUT")]
    [InlineData("/api/clothingitems/1", "DELETE")]
    [InlineData("/api/users", "POST")]
    [InlineData("/api/users/1", "PUT")]
    [InlineData("/api/users/1", "DELETE")]
    public async Task AdminEndpoints_WithUserToken_ReturnsForbidden(string endpoint, string method)
    {
        await using var factory = await _fixture.CreateFactoryAsync();
        var client = factory.CreateClient();

        var token = JwtTokenGenerator.GenerateUserToken();
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var request = new HttpRequestMessage(new HttpMethod(method), endpoint)
        {
            Content = method is "POST" or "PUT"
                ? JsonContent.Create(new { Name = "Test" })
                : null
        };

        var response = await client.SendAsync(request);

        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
    }

    [Theory]
    [InlineData("/api/users")]
    [InlineData("/api/users/1")]
    [InlineData("/api/roles")]
    [InlineData("/api/roles/1")]
    public async Task AdminOnlyGetEndpoints_WithUserToken_ReturnsForbidden(string endpoint)
    {
        await using var factory = await _fixture.CreateFactoryAsync();
        var client = factory.CreateClient();

        var token = JwtTokenGenerator.GenerateUserToken();
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var response = await client.GetAsync(endpoint);

        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
    }


    [Theory]
    [InlineData("/api/brands")]
    [InlineData("/api/categories")]
    [InlineData("/api/clothingitems")]
    [InlineData("/api/outfits")]
    [InlineData("/api/users")]
    [InlineData("/api/roles")]
    public async Task GetAllEndpoints_WithAdminToken_ReturnsOk(string endpoint)
    {
        await using var factory = await _fixture.CreateFactoryAsync();
        var client = factory.CreateClient();

        using var scope = factory.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        await TestDataSeeder.SeedRolesAsync(context);
        var adminUser = await TestDataSeeder.SeedUserAsync(
            context,
            $"Admin_{Guid.NewGuid().ToString("N")[..6]}",
            $"adm_{Guid.NewGuid().ToString("N")[..6]}@test.com",
            "Pass123!",
            "Admin");

        var token = JwtTokenGenerator.GenerateToken(adminUser.Id, adminUser.Email, "Admin");
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var response = await client.GetAsync(endpoint);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }


    [Fact]
    public async Task ProtectedEndpoint_WithTokenForNonExistentUser_ReturnsOk()
    {
        await using var factory = await _fixture.CreateFactoryAsync();
        var client = factory.CreateClient();

        var token = JwtTokenGenerator.GenerateToken(99999, "ghost@test.com", "User");
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var response = await client.GetAsync("/api/brands");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }


    [Fact]
    public async Task UserCannotSeeOtherUsersClothingItems()
    {
        await using var factory = await _fixture.CreateFactoryAsync();
        var client = factory.CreateClient();

        using var scope = factory.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        await TestDataSeeder.SeedRolesAsync(context);

        var user1 = await TestDataSeeder.SeedUserAsync(
            context,
            $"User1_{Guid.NewGuid().ToString("N")[..6]}",
            $"u1_{Guid.NewGuid().ToString("N")[..6]}@test.com",
            "Pass123!");

        var user2 = await TestDataSeeder.SeedUserAsync(
            context,
            $"User2_{Guid.NewGuid().ToString("N")[..6]}",
            $"u2_{Guid.NewGuid().ToString("N")[..6]}@test.com",
            "Pass123!");

        var category = await TestDataSeeder.SeedCategoryAsync(
            context,
            $"Cat_{Guid.NewGuid().ToString("N")[..6]}");

        var brand = await TestDataSeeder.SeedBrandAsync(
            context,
            $"Brand_{Guid.NewGuid().ToString("N")[..6]}");

        await TestDataSeeder.SeedClothingItemAsync(
            context, user1.Id, category.Id, brand.Id, "User1 Item", 10.00m);

        await TestDataSeeder.SeedClothingItemAsync(
            context, user2.Id, category.Id, brand.Id, "User2 Item", 20.00m);

        var token = JwtTokenGenerator.GenerateToken(user1.Id, user1.Email, "User");
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var response = await client.GetAsync("/api/clothingitems");

        var items = await response.Content.ReadFromJsonAsync<List<ClothingItemDto>>();
        Assert.NotNull(items);
        Assert.Single(items);
        Assert.Equal("User1 Item", items[0].Name);
    }

    [Fact]
    public async Task UserCannotAccessOtherUsersOutfit()
    {
        await using var factory = await _fixture.CreateFactoryAsync();
        var client = factory.CreateClient();

        using var scope = factory.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        await TestDataSeeder.SeedRolesAsync(context);

        var owner = await TestDataSeeder.SeedUserAsync(
            context,
            $"Owner_{Guid.NewGuid().ToString("N")[..6]}",
            $"own_{Guid.NewGuid().ToString("N")[..6]}@test.com",
            "Pass123!");

        var intruder = await TestDataSeeder.SeedUserAsync(
            context,
            $"Intruder_{Guid.NewGuid().ToString("N")[..6]}",
            $"bad_{Guid.NewGuid().ToString("N")[..6]}@test.com",
            "Pass123!");

        var outfit = await TestDataSeeder.SeedOutfitAsync(
            context, owner.Id, $"Secret Outfit {Guid.NewGuid().ToString("N")[..6]}");

        var token = JwtTokenGenerator.GenerateToken(intruder.Id, intruder.Email, "User");
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var response = await client.GetAsync($"/api/outfits/{outfit.Id}");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task UserCannotAddItemToOtherUsersOutfit()
    {
        await using var factory = await _fixture.CreateFactoryAsync();
        var client = factory.CreateClient();

        using var scope = factory.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        await TestDataSeeder.SeedRolesAsync(context);

        var owner = await TestDataSeeder.SeedUserAsync(
            context,
            $"Owner_{Guid.NewGuid().ToString("N")[..6]}",
            $"own_{Guid.NewGuid().ToString("N")[..6]}@test.com",
            "Pass123!");

        var intruder = await TestDataSeeder.SeedUserAsync(
            context,
            $"Bad_{Guid.NewGuid().ToString("N")[..6]}",
            $"bad_{Guid.NewGuid().ToString("N")[..6]}@test.com",
            "Pass123!");

        var category = await TestDataSeeder.SeedCategoryAsync(
            context,
            $"Cat_{Guid.NewGuid().ToString("N")[..6]}");

        var brand = await TestDataSeeder.SeedBrandAsync(
            context,
            $"Brand_{Guid.NewGuid().ToString("N")[..6]}");

        var outfit = await TestDataSeeder.SeedOutfitAsync(
            context, owner.Id, $"Protected Outfit {Guid.NewGuid().ToString("N")[..6]}");

        var clothingItem = await TestDataSeeder.SeedClothingItemAsync(
            context, owner.Id, category.Id, brand.Id, $"Owner Item {Guid.NewGuid().ToString("N")[..6]}", 10.00m);

        var token = JwtTokenGenerator.GenerateToken(intruder.Id, intruder.Email, "User");
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var dto = new CreateOutfitItemDto
        {
            OutfitId = outfit.Id,
            ClothingItemId = clothingItem.Id
        };

        var response = await client.PostAsJsonAsync("/api/outfititems", dto);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }


    [Fact]
    public async Task ProtectedEndpoint_WithMalformedToken_ReturnsUnauthorized()
    {
        await using var factory = await _fixture.CreateFactoryAsync();
        var client = factory.CreateClient();

        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", "not-a-valid-jwt-token");

        var response = await client.GetAsync("/api/brands");

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }


    [Fact]
    public async Task AuthRegister_WithoutToken_ReturnsOk()
    {
        await using var factory = await _fixture.CreateFactoryAsync();
        var client = factory.CreateClient();

        using var scope = factory.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        await TestDataSeeder.SeedRolesAsync(context);

        var dto = new RegisterDto
        {
            Username = $"open_{Guid.NewGuid().ToString("N")[..6]}",
            Email = $"open_{Guid.NewGuid().ToString("N")[..6]}@test.com",
            Password = "Password123!"
        };

        var response = await client.PostAsJsonAsync("/api/auth/register", dto);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task AuthLogin_WithoutToken_ReturnsUnauthorizedOnBadCredentials()
    {
        await using var factory = await _fixture.CreateFactoryAsync();
        var client = factory.CreateClient();

        var dto = new LoginDto
        {
            Email = "nonexistent@test.com",
            Password = "WrongPass1!"
        };

        var response = await client.PostAsJsonAsync("/api/auth/login", dto);

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }
}