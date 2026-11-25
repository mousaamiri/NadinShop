using System.Net;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Mvc;
using NadinShop.IntegrationTests.TestSupport;

namespace NadinShop.IntegrationTests.Endpoints;

public class ExceptionHandlingMiddlewareTests(ApiFactory factory) : IClassFixture<ApiFactory>
{
    [Fact]
    public async Task DomainException_ReturnsBadRequestProblemDetails()
    {
        var client = factory.CreateClient();

        var response = await client.GetAsync("/api/test/errors/domain");

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        var problem = await response.Content.ReadFromJsonAsync<ProblemDetails>();
        Assert.NotNull(problem);
        Assert.Equal("Domain rule was violated.", problem!.Detail);
        Assert.Equal("Error.Domain", problem.Extensions["code"]?.ToString());
    }

    [Fact]
    public async Task UnhandledException_ReturnsInternalServerErrorWithoutLeakingDetails()
    {
        var client = factory.CreateClient();

        var response = await client.GetAsync("/api/test/errors/unhandled");

        Assert.Equal(HttpStatusCode.InternalServerError, response.StatusCode);
        var problem = await response.Content.ReadFromJsonAsync<ProblemDetails>();
        Assert.NotNull(problem);
        Assert.Equal("Error.InternalServerError", problem!.Extensions["code"]?.ToString());
        Assert.DoesNotContain("Sensitive internal failure", problem.Detail);
    }
}
