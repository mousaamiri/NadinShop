using NadinShop.Application.Common.Messaging;
using NadinShop.Application.Common.Services;

namespace NadinShop.Application.Features.Identity.Queries.GetCurrentUser;

public record GetCurrentUserQuery : IQuery<CurrentUser>;
