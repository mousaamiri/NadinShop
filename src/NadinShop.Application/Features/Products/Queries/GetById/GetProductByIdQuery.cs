using NadinShop.Application.Common.Messaging;
using NadinShop.Application.Features.Products.Dtos;

namespace NadinShop.Application.Features.Products.Queries.GetById;

public record GetProductByIdQuery(Guid Id) : IQuery<ProductDto>;
