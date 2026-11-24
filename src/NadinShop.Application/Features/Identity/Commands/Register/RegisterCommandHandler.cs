using NadinShop.Application.Common.Messaging;
using NadinShop.Application.Common.ResultPattern;
using NadinShop.Application.Common.Services;

namespace NadinShop.Application.Features.Identity.Commands.Register;

public class RegisterCommandHandler(IIdentityService identityService) : ICommandHandler<RegisterCommand, Guid>
{
    public Task<Result<Guid>> Handle(RegisterCommand request, CancellationToken cancellationToken)
        => identityService.CreateUserAsync(request.Email, request.Phone, request.Password, cancellationToken);
}
