using System.Net.Http.Headers;
using System.Net.Http.Json;
using NadinShop.Application.Features.Identity.Commands.Dtos;

namespace NadinShop.IntegrationTests.TestSupport;

public sealed record TestUser(string Email, string Phone, string Password, string AccessToken);

public static class ApiClientExtensions
{
    public const string DefaultPassword = "Password123!";

    public static async Task<TestUser> RegisterAndLoginAsync(
        this HttpClient client,
        string? email = null,
        string? phone = null,
        string password = DefaultPassword)
    {
        email ??= $"user-{Guid.NewGuid():N}@example.com";
        phone ??= CreatePhoneNumber();

        var registerResponse = await client.PostAsJsonAsync(
            "/api/users/register",
            new { email, phone, password });
        registerResponse.EnsureSuccessStatusCode();

        var loginResponse = await client.PostAsJsonAsync(
            "/api/users/login",
            new { email, password });
        loginResponse.EnsureSuccessStatusCode();

        var auth = await loginResponse.Content.ReadFromJsonAsync<AuthResponse>();
        Assert.NotNull(auth);
        Assert.False(string.IsNullOrWhiteSpace(auth!.AccessToken));

        return new TestUser(email, phone, password, auth.AccessToken);
    }

    public static HttpClient CreateAuthenticatedClient(this ApiFactory factory, string accessToken)
    {
        var client = factory.CreateClient();
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
        return client;
    }

    public static string CreatePhoneNumber()
        => $"09{Random.Shared.Next(100_000_000, 999_999_999)}";
}
