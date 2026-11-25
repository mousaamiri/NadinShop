using System.Net;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Mvc;
using NadinShop.Application.Common.Services;
using NadinShop.Application.Features.Identity.Commands.Dtos;
using NadinShop.IntegrationTests.TestSupport;

namespace NadinShop.IntegrationTests.Endpoints;

public class UsersEndpointsTests(ApiFactory factory) : IClassFixture<ApiFactory>
{
    [Fact]
    public async Task Register_WithValidRequest_ReturnsCreated()
    {
        var client = factory.CreateClient();

        var response = await client.PostAsJsonAsync(
            "/api/users/register",
            new
            {
                email = UniqueEmail(),
                phone = ApiClientExtensions.CreatePhoneNumber(),
                password = ApiClientExtensions.DefaultPassword
            });

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
    }

    [Fact]
    public async Task Register_WithDuplicateEmail_ReturnsConflict()
    {
        var client = factory.CreateClient();
        var email = UniqueEmail();

        var firstResponse = await client.PostAsJsonAsync(
            "/api/users/register",
            new
            {
                email,
                phone = ApiClientExtensions.CreatePhoneNumber(),
                password = ApiClientExtensions.DefaultPassword
            });
        firstResponse.EnsureSuccessStatusCode();

        var response = await client.PostAsJsonAsync(
            "/api/users/register",
            new
            {
                email,
                phone = ApiClientExtensions.CreatePhoneNumber(),
                password = ApiClientExtensions.DefaultPassword
            });

        Assert.Equal(HttpStatusCode.Conflict, response.StatusCode);
        var problem = await response.Content.ReadFromJsonAsync<ProblemDetails>();
        Assert.Equal("Error.EmailAlreadyExists", problem!.Extensions["code"]?.ToString());
    }

    [Fact]
    public async Task Register_WithInvalidEmail_ReturnsValidationProblem()
    {
        var client = factory.CreateClient();

        var response = await client.PostAsJsonAsync(
            "/api/users/register",
            new
            {
                email = "not-an-email",
                phone = ApiClientExtensions.CreatePhoneNumber(),
                password = ApiClientExtensions.DefaultPassword
            });

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        var problem = await response.Content.ReadFromJsonAsync<ProblemDetails>();
        Assert.StartsWith(
            "RegisterCommand.Email",
            problem!.Extensions["code"]?.ToString(),
            StringComparison.Ordinal);
    }

    [Fact]
    public async Task Login_WithValidCredentials_ReturnsToken()
    {
        var user = await factory.CreateClient().RegisterAndLoginAsync();

        Assert.False(string.IsNullOrWhiteSpace(user.AccessToken));
    }

    [Fact]
    public async Task Login_WithInvalidPassword_ReturnsUnauthorized()
    {
        var client = factory.CreateClient();
        var email = UniqueEmail();
        var phone = ApiClientExtensions.CreatePhoneNumber();

        var registerResponse = await client.PostAsJsonAsync(
            "/api/users/register",
            new { email, phone, password = ApiClientExtensions.DefaultPassword });
        registerResponse.EnsureSuccessStatusCode();

        var response = await client.PostAsJsonAsync(
            "/api/users/login",
            new { email, password = "WrongPassword123!" });

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        var problem = await response.Content.ReadFromJsonAsync<ProblemDetails>();
        Assert.Equal("Error.InvalidCredentials", problem!.Extensions["code"]?.ToString());
    }

    [Fact]
    public async Task Login_WithUnknownEmail_ReturnsUnauthorized()
    {
        var client = factory.CreateClient();

        var response = await client.PostAsJsonAsync(
            "/api/users/login",
            new { email = UniqueEmail(), password = ApiClientExtensions.DefaultPassword });

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task GetCurrentUser_WithoutToken_ReturnsUnauthorized()
    {
        var client = factory.CreateClient();

        var response = await client.GetAsync("/api/users/me");

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task GetCurrentUser_WithToken_ReturnsProfile()
    {
        var user = await factory.CreateClient().RegisterAndLoginAsync();
        var client = factory.CreateAuthenticatedClient(user.AccessToken);

        var response = await client.GetAsync("/api/users/me");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var currentUser = await response.Content.ReadFromJsonAsync<CurrentUser>();
        Assert.NotNull(currentUser);
        Assert.Equal(user.Email, currentUser!.Email);
        Assert.Equal(user.Phone, currentUser.Phone);
    }

    [Fact]
    public async Task Login_ResponseContainsExpectedTokenShape()
    {
        var client = factory.CreateClient();
        var email = UniqueEmail();
        var phone = ApiClientExtensions.CreatePhoneNumber();

        var registerResponse = await client.PostAsJsonAsync(
            "/api/users/register",
            new { email, phone, password = ApiClientExtensions.DefaultPassword });
        registerResponse.EnsureSuccessStatusCode();

        var response = await client.PostAsJsonAsync(
            "/api/users/login",
            new { email, password = ApiClientExtensions.DefaultPassword });

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var auth = await response.Content.ReadFromJsonAsync<AuthResponse>();
        Assert.NotNull(auth);
        Assert.False(string.IsNullOrWhiteSpace(auth!.AccessToken));
        Assert.False(string.IsNullOrWhiteSpace(auth.RefreshToken));
        Assert.True(auth.ExpirationDate > DateTime.UtcNow);
    }

    private static string UniqueEmail()
        => $"user-{Guid.NewGuid():N}@example.com";
}
