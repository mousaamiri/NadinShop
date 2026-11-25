using Moq;
using NadinShop.Application.Common.Interfaces;
using NadinShop.Application.Common.ResultPattern;
using NadinShop.Application.Features.Products.Queries.GetById;
using NadinShop.Domain.Entities.Products;

namespace NadinShop.Application.Tests.Features.Products.Queries.GetById;

public class GetProductByIdQueryHandlerTests
{
    private readonly Product _product;
    private readonly GetProductByIdQuery _query;
    private readonly CancellationToken _token;
    private readonly Mock<IProductRepository> _productRepository;
    private readonly GetProductByIdQueryHandler _handler;

    public GetProductByIdQueryHandlerTests()
    {
        using var cancellationTokenSource = new CancellationTokenSource();
        _token = cancellationTokenSource.Token;
        _product = Product.Create("Galaxy S24", "09121234567", "Test@Example.com", DateTime.UtcNow, isAvailable: true);
        _query = new GetProductByIdQuery(_product.Id);

        _productRepository = new Mock<IProductRepository>();
        _productRepository
            .Setup(r => r.GetByIdAsync(_product.Id, _token))
            .ReturnsAsync(_product);

        _handler = new GetProductByIdQueryHandler(_productRepository.Object);
    }

    [Fact]
    public async Task Handle_WhenProductExists_ReturnsMappedProduct()
    {
        var result = await _handler.Handle(_query, _token);

        Assert.True(result.IsSuccess);
        Assert.Equal(_product.Id, result.Value.Id);
        Assert.Equal(_product.Name.Value, result.Value.Name);
        Assert.Equal(_product.ManufacturePhone.Value, result.Value.ManufacturePhone);
        Assert.Equal(_product.ManufactureEmail.Value, result.Value.ManufactureEmail);
        Assert.Equal(_product.ProduceDate.Value, result.Value.ProduceDate);
        Assert.Equal(_product.IsAvailable, result.Value.IsAvailable);
        _productRepository.Verify(r => r.GetByIdAsync(_product.Id, _token), Times.Once);
        _productRepository.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task Handle_WhenProductDoesNotExist_ReturnsNotFound()
    {
        _productRepository
            .Setup(r => r.GetByIdAsync(_query.Id, _token))
            .ReturnsAsync((Product?)null);

        var result = await _handler.Handle(_query, _token);

        Assert.True(result.IsFailure);
        Assert.Equal(ErrorType.NotFound, result.Error.ErrorType);
        Assert.Equal("Error.ProductNotFound", result.Error.Code);
    }

    [Fact]
    public async Task Handle_WhenProductDoesNotExist_DoesNotExposeValue()
    {
        _productRepository
            .Setup(r => r.GetByIdAsync(_query.Id, _token))
            .ReturnsAsync((Product?)null);

        var result = await _handler.Handle(_query, _token);

        Assert.Throws<InvalidOperationException>(() => _ = result.Value);
    }
}
