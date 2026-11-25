using System.Text;
using Microsoft.Extensions.Configuration;

namespace NadinShop.Infrastructure.Services;

public sealed class JwtOptions
{
    public const string SectionName = "Jwt";

    public const int MinimumSigningKeyLengthInBytes = 32;

    public string Issuer { get; init; } = string.Empty;

    public string Audience { get; init; } = string.Empty;

    public string SigningKey { get; init; } = string.Empty;

    public int AccessTokenMinutes { get; init; } = 60;

    public static JwtOptions FromConfiguration(IConfiguration configuration)
    {
        ArgumentNullException.ThrowIfNull(configuration);

        var section = configuration.GetSection(SectionName);

        return new JwtOptions
        {
            Issuer = section["Issuer"] ?? string.Empty,
            Audience = section["Audience"] ?? string.Empty,
            SigningKey = section["SigningKey"] ?? string.Empty,
            AccessTokenMinutes = int.TryParse(section["AccessTokenMinutes"], out var minutes)
                ? minutes
                : 60
        };
    }

    public void Validate()
    {
        if (string.IsNullOrWhiteSpace(Issuer))
            throw new InvalidOperationException($"JWT issuer is not configured ('{SectionName}:Issuer').");

        if (string.IsNullOrWhiteSpace(Audience))
            throw new InvalidOperationException($"JWT audience is not configured ('{SectionName}:Audience').");

        if (string.IsNullOrWhiteSpace(SigningKey)
            || Encoding.UTF8.GetByteCount(SigningKey) < MinimumSigningKeyLengthInBytes)
        {
            throw new InvalidOperationException(
                $"JWT signing key ('{SectionName}:SigningKey') must be at least {MinimumSigningKeyLengthInBytes} bytes long.");
        }

        if (AccessTokenMinutes <= 0)
            throw new InvalidOperationException($"JWT access token lifetime ('{SectionName}:AccessTokenMinutes') must be greater than zero.");
    }
}
