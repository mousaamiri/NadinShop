namespace NadinShop.Application.Features.Products.Dtos;

public record ProductDto(
    Guid Id,
    string Name,
    string ManufacturePhone,
    string ManufactureEmail,
    DateTime ProduceDate,
    bool IsAvailable);
