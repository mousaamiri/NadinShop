using NadinShop.Application.Common.Messaging;
using NadinShop.Application.Features.Products.Dtos;

namespace NadinShop.Application.Features.Products.Queries.GetByManufactureEmail;

public record GetProductsByManufactureEmailQuery(string ManufactureEmail)
    : IQuery<IReadOnlyList<ProductDto>>;
