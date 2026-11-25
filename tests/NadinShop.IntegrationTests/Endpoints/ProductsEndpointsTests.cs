using System.Net;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Mvc;
using NadinShop.Application.Features.Products.Commands.Create;
using NadinShop.Application.Features.Products.Commands.Update;
using NadinShop.Application.Features.Products.Dtos;
using NadinShop.IntegrationTests.TestSupport;

namespace NadinShop.IntegrationTests.Endpoints;

public class ProductsEndpointsTests(ApiFactory factory) : IClassFixture<ApiFactory>
{
    [Fact]
    public async Task GetProductById_WithUnknownId_ReturnsNotFound()
    {
        var client = factory.CreateClient();

        var response = await client.GetAsync($"/api/products/{Guid.NewGuid()}");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        var problem = await response.Content.ReadFromJsonAsync<ProblemDetails>();
        Assert.Equal("Error.ProductNotFound", problem!.Extensions["code"]?.ToString());
    }

    [Fact]
    public async Task CreateProduct_WithoutToken_ReturnsUnauthorized()
    {
        var client = factory.CreateClient();

        var response = await client.PostAsJsonAsync(
            "/api/products",
            new { name = UniqueProductName(), produceDate = DateTime.UtcNow });

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task CreateProduct_WithInvalidName_ReturnsValidationProblem()
    {
        var user = await factory.CreateClient().RegisterAndLoginAsync();
        var client = factory.CreateAuthenticatedClient(user.AccessToken);

        var response = await client.PostAsJsonAsync(
            "/api/products",
            new { name = "1", produceDate = DateTime.UtcNow });

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        var problem = await response.Content.ReadFromJsonAsync<ProblemDetails>();
        Assert.StartsWith(
            "CreateProductCommand.Name",
            problem!.Extensions["code"]?.ToString(),
            StringComparison.Ordinal);
    }

    [Fact]
    public async Task CreateProduct_ThenGetById_ReturnsCreatedProduct()
    {
        var user = await factory.CreateClient().RegisterAndLoginAsync();
        var client = factory.CreateAuthenticatedClient(user.AccessToken);
        var name = UniqueProductName();

        var createResponse = await client.PostAsJsonAsync(
            "/api/products",
            new { name, produceDate = DateTime.UtcNow });

        Assert.Equal(HttpStatusCode.Created, createResponse.StatusCode);
        var created = await createResponse.Content.ReadFromJsonAsync<CreateProductResponse>();
        Assert.NotNull(created);
        Assert.Equal(name, created!.Name);
        Assert.Equal(user.Phone, created.ManufacturePhone);
        Assert.Equal(user.Email, created.ManufactureEmail);

        var getResponse = await client.GetAsync($"/api/products/{created.Id}");

        Assert.Equal(HttpStatusCode.OK, getResponse.StatusCode);
        var product = await getResponse.Content.ReadFromJsonAsync<ProductDto>();
        Assert.NotNull(product);
        Assert.Equal(created.Id, product!.Id);
        Assert.Equal(name, product.Name);
        Assert.True(product.IsAvailable);
    }

    [Fact]
    public async Task GetProducts_ReturnsCreatedProduct()
    {
        var user = await factory.CreateClient().RegisterAndLoginAsync();
        var client = factory.CreateAuthenticatedClient(user.AccessToken);
        var created = await CreateProductAsync(client);

        var response = await client.GetAsync("/api/products");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var products = await response.Content.ReadFromJsonAsync<List<ProductDto>>();
        Assert.NotNull(products);
        Assert.Contains(products!, product => product.Id == created.Id);
    }

    [Fact]
    public async Task GetProductsByManufactureEmail_ReturnsOnlyMatchingProducts()
    {
        var owner = await factory.CreateClient().RegisterAndLoginAsync();
        var ownerClient = factory.CreateAuthenticatedClient(owner.AccessToken);
        var ownedProduct = await CreateProductAsync(ownerClient);

        var otherUser = await factory.CreateClient().RegisterAndLoginAsync();
        var otherClient = factory.CreateAuthenticatedClient(otherUser.AccessToken);
        var otherProduct = await CreateProductAsync(otherClient);

        var response = await ownerClient.GetAsync(
            $"/api/products/by-manufacture-email?manufactureEmail={Uri.EscapeDataString(owner.Email)}");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var products = await response.Content.ReadFromJsonAsync<List<ProductDto>>();
        Assert.NotNull(products);
        var product = Assert.Single(products!);
        Assert.Equal(ownedProduct.Id, product.Id);
        Assert.NotEqual(otherProduct.Id, product.Id);
        Assert.Equal(owner.Email, product.ManufactureEmail);
    }

    [Fact]
    public async Task UpdateProduct_ByOwner_ReturnsUpdatedProduct()
    {
        var user = await factory.CreateClient().RegisterAndLoginAsync();
        var client = factory.CreateAuthenticatedClient(user.AccessToken);
        var created = await CreateProductAsync(client);
        var newName = UniqueProductName();

        var response = await client.PutAsJsonAsync(
            $"/api/products/{created.Id}",
            new { name = newName, isAvailable = false });

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var updated = await response.Content.ReadFromJsonAsync<UpdateProductResponse>();
        Assert.NotNull(updated);
        Assert.Equal(created.Id, updated!.Id);
        Assert.Equal(newName, updated.Name);
        Assert.False(updated.IsAvailable);
    }

    [Fact]
    public async Task UpdateProduct_ByAnotherUser_ReturnsForbidden()
    {
        var owner = await factory.CreateClient().RegisterAndLoginAsync();
        var ownerClient = factory.CreateAuthenticatedClient(owner.AccessToken);
        var created = await CreateProductAsync(ownerClient);

        var otherUser = await factory.CreateClient().RegisterAndLoginAsync();
        var otherClient = factory.CreateAuthenticatedClient(otherUser.AccessToken);

        var response = await otherClient.PutAsJsonAsync(
            $"/api/products/{created.Id}",
            new { name = UniqueProductName(), isAvailable = true });

        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
        var problem = await response.Content.ReadFromJsonAsync<ProblemDetails>();
        Assert.Equal("Error.ProductAccessDenied", problem!.Extensions["code"]?.ToString());
    }

    [Fact]
    public async Task DeleteProduct_ByOwner_ReturnsNoContentAndRemovesProduct()
    {
        var user = await factory.CreateClient().RegisterAndLoginAsync();
        var client = factory.CreateAuthenticatedClient(user.AccessToken);
        var created = await CreateProductAsync(client);

        var deleteResponse = await client.DeleteAsync($"/api/products/{created.Id}");

        Assert.Equal(HttpStatusCode.NoContent, deleteResponse.StatusCode);

        var getResponse = await client.GetAsync($"/api/products/{created.Id}");
        Assert.Equal(HttpStatusCode.NotFound, getResponse.StatusCode);
    }

    [Fact]
    public async Task DeleteProduct_ByAnotherUser_ReturnsForbidden()
    {
        var owner = await factory.CreateClient().RegisterAndLoginAsync();
        var created = await CreateProductAsync(factory.CreateAuthenticatedClient(owner.AccessToken));

        var otherUser = await factory.CreateClient().RegisterAndLoginAsync();
        var otherClient = factory.CreateAuthenticatedClient(otherUser.AccessToken);

        var response = await otherClient.DeleteAsync($"/api/products/{created.Id}");

        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
    }

    private static string UniqueProductName()
        => $"Product {Guid.NewGuid():N}";

    private static async Task<CreateProductResponse> CreateProductAsync(HttpClient client)
    {
        var response = await client.PostAsJsonAsync(
            "/api/products",
            new { name = UniqueProductName(), produceDate = DateTime.UtcNow });

        response.EnsureSuccessStatusCode();

        var created = await response.Content.ReadFromJsonAsync<CreateProductResponse>();
        Assert.NotNull(created);
        return created!;
    }
}
