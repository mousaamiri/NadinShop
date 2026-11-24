using NadinShop.Application.Common.Messaging;

namespace NadinShop.Application.Features.Identity.Commands.Register;

public record RegisterCommand(string Email,string Phone, string Password):ICommand<Guid>;
