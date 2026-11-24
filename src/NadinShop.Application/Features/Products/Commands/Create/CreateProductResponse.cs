namespace NadinShop.Application.Features.Products.Commands.Create;

public record CreateProductResponse(Guid Id,string Name,string ManufacturePhone,string ManufactureEmail,DateTime ProduceDate);
