namespace NadinShop.Application.Features.Products.Commands.Update;

public record UpdateProductResponse(
    Guid Id,
    string Name,
    string ManufacturePhone,
    string ManufactureEmail,
    bool IsAvailable);
