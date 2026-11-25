namespace NadinShop.Api.Contracts;

public sealed record CreateProductRequest(string Name, DateTime ProduceDate);

public sealed record UpdateProductRequest(string Name, bool IsAvailable);
