using Moq;
using NadinShop.Application.Common.Interfaces;
using NadinShop.Application.Features.Products.Queries.GetByManufactureEmail;
using NadinShop.Domain.Entities.Products;

namespace NadinShop.Application.Tests.Features.Products.Queries.GetByManufactureEmail;

public class GetProductsByManufactureEmailQueryHandlerTests
{
    private const string ManufactureEmail = "Test@Example.com";

    private readonly GetProductsByManufactureEmailQuery _query;
    private readonly CancellationToken _token;
    private readonly Mock<IProductRepository> _productRepository;
    private readonly GetProductsByManufactureEmailQueryHandler _handler;

    public GetProductsByManufactureEmailQueryHandlerTests()
    {
        using var cancellationTokenSource = new CancellationTokenSource();
        _token = cancellationTokenSource.Token;
        _query = new GetProductsByManufactureEmailQuery(ManufactureEmail);

        _productRepository = new Mock<IProductRepository>();
        _handler = new GetProductsByManufactureEmailQueryHandler(_productRepository.Object);
    }

    [Fact]
    public async Task Handle_WhenMatchingProductsExist_ReturnsMappedProducts()
    {
        var product = Product.Create("Galaxy S24", "09121234567", ManufactureEmail, DateTime.UtcNow, isAvailable: true);
        _productRepository
            .Setup(r => r.ListByManufactureEmailAsync(_query.ManufactureEmail, _token))
            .ReturnsAsync([product]);

        var result = await _handler.Handle(_query, _token);

        Assert.True(result.IsSuccess);
        var dto = Assert.Single(result.Value);
        Assert.Equal(product.Id, dto.Id);
        Assert.Equal(product.Name.Value, dto.Name);
        Assert.Equal(product.ManufactureEmail.Value, dto.ManufactureEmail);
        Assert.Equal(product.IsAvailable, dto.IsAvailable);
        _productRepository.Verify(r => r.ListByManufactureEmailAsync(_query.ManufactureEmail, _token), Times.Once);
        _productRepository.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task Handle_WhenNoProductsMatch_ReturnsEmptyList()
    {
        _productRepository
            .Setup(r => r.ListByManufactureEmailAsync(_query.ManufactureEmail, _token))
            .ReturnsAsync([]);

        var result = await _handler.Handle(_query, _token);

        Assert.True(result.IsSuccess);
        Assert.Empty(result.Value);
        _productRepository.Verify(r => r.ListByManufactureEmailAsync(_query.ManufactureEmail, _token), Times.Once);
        _productRepository.VerifyNoOtherCalls();
    }
}
