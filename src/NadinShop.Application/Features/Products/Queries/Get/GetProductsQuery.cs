using NadinShop.Application.Common.Messaging;
using NadinShop.Application.Features.Products.Dtos;

namespace NadinShop.Application.Features.Products.Queries.Get;

public record GetProductsQuery : IQuery<IReadOnlyList<ProductDto>>;
