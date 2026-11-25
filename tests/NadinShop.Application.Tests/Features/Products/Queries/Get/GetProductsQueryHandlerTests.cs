using Moq;
using NadinShop.Application.Common.Interfaces;
using NadinShop.Application.Features.Products.Queries.Get;
using NadinShop.Domain.Entities.Products;

namespace NadinShop.Application.Tests.Features.Products.Queries.Get;

public class GetProductsQueryHandlerTests
{
    private readonly CancellationToken _token;
    private readonly Mock<IProductRepository> _productRepository;
    private readonly GetProductsQueryHandler _handler;

    public GetProductsQueryHandlerTests()
    {
        using var cancellationTokenSource = new CancellationTokenSource();
        _token = cancellationTokenSource.Token;

        _productRepository = new Mock<IProductRepository>();
        _handler = new GetProductsQueryHandler(_productRepository.Object);
    }

    [Fact]
    public async Task Handle_WhenProductsExist_ReturnsMappedProducts()
    {
        var firstProduct = Product.Create("Galaxy S24", "09121234567", "Test@Example.com", isAvailable: true);
        var secondProduct = Product.Create("iPhone 15 Pro", "09351234567", "Support@NadinShop.COM", isAvailable: false);
        _productRepository
            .Setup(r => r.ListAsync(_token))
            .ReturnsAsync([firstProduct, secondProduct]);

        var result = await _handler.Handle(new GetProductsQuery(), _token);

        Assert.True(result.IsSuccess);
        Assert.Collection(
            result.Value,
            first =>
            {
                Assert.Equal(firstProduct.Id, first.Id);
                Assert.Equal(firstProduct.Name.Value, first.Name);
                Assert.Equal(firstProduct.ManufacturePhone.Value, first.ManufacturePhone);
                Assert.Equal(firstProduct.ManufactureEmail.Value, first.ManufactureEmail);
                Assert.Equal(firstProduct.ProduceDate.Value, first.ProduceDate);
                Assert.Equal(firstProduct.IsAvailable, first.IsAvailable);
            },
            second =>
            {
                Assert.Equal(secondProduct.Id, second.Id);
                Assert.Equal(secondProduct.Name.Value, second.Name);
                Assert.Equal(secondProduct.IsAvailable, second.IsAvailable);
            });
        _productRepository.Verify(r => r.ListAsync(_token), Times.Once);
        _productRepository.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task Handle_WhenNoProductsExist_ReturnsEmptyList()
    {
        _productRepository
            .Setup(r => r.ListAsync(_token))
            .ReturnsAsync([]);

        var result = await _handler.Handle(new GetProductsQuery(), _token);

        Assert.True(result.IsSuccess);
        Assert.Empty(result.Value);
        _productRepository.Verify(r => r.ListAsync(_token), Times.Once);
        _productRepository.VerifyNoOtherCalls();
    }
}
