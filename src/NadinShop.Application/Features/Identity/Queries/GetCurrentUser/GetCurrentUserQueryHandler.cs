using NadinShop.Application.Common.Messaging;
using NadinShop.Application.Common.ResultPattern;
using NadinShop.Application.Common.Services;

namespace NadinShop.Application.Features.Identity.Queries.GetCurrentUser;

public class GetCurrentUserQueryHandler(ICurrentUserService currentUserService)
    : IQueryHandler<GetCurrentUserQuery, CurrentUser>
{
    public async Task<Result<CurrentUser>> Handle(
        GetCurrentUserQuery request,
        CancellationToken cancellationToken)
    {
        var currentUser = await currentUserService.GetCurrentUserAsync(cancellationToken);
        if (currentUser is null)
            return Error.Unauthorized("Error.Unauthenticated", "User is not authenticated.");

        return currentUser;
    }
}
