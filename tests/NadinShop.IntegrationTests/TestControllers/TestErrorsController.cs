using System.Diagnostics.CodeAnalysis;
using Microsoft.AspNetCore.Mvc;
using NadinShop.Domain.Common;

namespace NadinShop.IntegrationTests.TestControllers;

[ApiController]
[Route("api/test/errors")]
[SuppressMessage(
    "Performance",
    "CA1822:Mark members as static",
    Justification = "MVC action methods must be instance methods.")]
public sealed class TestErrorsController : ControllerBase
{
    [HttpGet("domain")]
    public IActionResult ThrowDomainException()
        => throw new DomainException("Domain rule was violated.");

    [HttpGet("unhandled")]
    public IActionResult ThrowUnhandledException()
        => throw new InvalidOperationException("Sensitive internal failure.");
}
