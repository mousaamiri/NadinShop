using NadinShop.Application.Common.Messaging;
using NadinShop.Application.Common.ResultPattern;
using NadinShop.Application.Common.Services;

namespace NadinShop.Application.Features.Identity.Commands.Login;

public class LoginCommandHandler(IIdentityService identityService, IJwTokenService jwtTokenService)
    : ICommandHandler<LoginCommand, JwtResult>
{
    public async Task<Result<JwtResult>> Handle(LoginCommand request, CancellationToken cancellationToken)
    {
        var authUserResult = await identityService.ValidateCredentialsAsync(
            request.Email,
            request.Password,
            cancellationToken);

        if (authUserResult.IsFailure)
            return authUserResult.Error;

        var jwtResult = await jwtTokenService.GenerateAccessToken(authUserResult.Value, cancellationToken);
        return jwtResult;
    }
}
