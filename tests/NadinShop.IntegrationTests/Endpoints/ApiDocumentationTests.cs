using System.Net;
using System.Text.Json;
using NadinShop.IntegrationTests.TestSupport;

namespace NadinShop.IntegrationTests.Endpoints;

public class ApiDocumentationTests(ApiFactory factory) : IClassFixture<ApiFactory>
{
    [Fact]
    public async Task SwaggerDocument_IsServedWithBearerSecurityScheme()
    {
        var client = factory.CreateClient();

        var response = await client.GetAsync("/swagger/v1/swagger.json");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        using var document = JsonDocument.Parse(await response.Content.ReadAsStringAsync());
        var root = document.RootElement;

        Assert.Equal("NadinShop API", root.GetProperty("info").GetProperty("title").GetString());
        Assert.True(root.GetProperty("paths").TryGetProperty("/api/products", out _));
        Assert.True(root.GetProperty("paths").TryGetProperty("/api/users/login", out _));
        Assert.True(
            root.GetProperty("components")
                .GetProperty("securitySchemes")
                .TryGetProperty("Bearer", out _));
    }

    [Fact]
    public async Task ScalarReference_IsServed()
    {
        var client = factory.CreateClient();

        var response = await client.GetAsync("/scalar/v1");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var html = await response.Content.ReadAsStringAsync();
        Assert.Contains("NadinShop API", html, StringComparison.Ordinal);
    }
}
