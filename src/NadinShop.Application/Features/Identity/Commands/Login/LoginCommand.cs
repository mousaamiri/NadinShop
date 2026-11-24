using NadinShop.Application.Common.Messaging;
using NadinShop.Application.Common.Services;

namespace NadinShop.Application.Features.Identity.Commands.Login;

public record LoginCommand(string Email,string Password):ICommand<JwtResult>;
