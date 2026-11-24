using Moq;
using NadinShop.Application.Common.Interfaces;
using NadinShop.Application.Common.ResultPattern;
using NadinShop.Application.Common.Services;
using NadinShop.Application.Features.Products.Commands.Create;
using NadinShop.Domain.Entities.Products;

namespace NadinShop.Application.Tests.Features.Products.Commands.Create;

public class CreateProductCommandHandlerTests
{
    private const string ValidName = "Galaxy S24";
    private const string ValidPhone = "09121234567";
    private const string ValidEmail = "Test@Example.com";

    private readonly CreateProductCommand _command;
    private readonly CancellationToken _token;
    private readonly CurrentUser _currentUser;
    private readonly Mock<IProductRepository> _productRepository;
    private readonly Mock<ICurrentUserService> _currentUserService;
    private readonly CreateProductCommandHandler _handler;

    public CreateProductCommandHandlerTests()
    {
        _command = new CreateProductCommand(ValidName, DateTime.UtcNow);
        using var cancellationTokenSource = new CancellationTokenSource();
        _token = cancellationTokenSource.Token;
        _currentUser = new CurrentUser(Guid.NewGuid(), "test@example.com", ValidPhone);

        _productRepository = new Mock<IProductRepository>();
        _currentUserService = new Mock<ICurrentUserService>();
        _currentUserService
            .Setup(s => s.GetCurrentUserAsync(_token))
            .ReturnsAsync(_currentUser);
        _productRepository
            .Setup(r => r.GetByNameAsync(_command.Name, _token))
            .ReturnsAsync((Product?)null);

        _handler = new CreateProductCommandHandler(_productRepository.Object, _currentUserService.Object);
    }

    [Fact]
    public async Task Handle_WithCurrentUserContact_AddsProduct()
    {
        Product? addedProduct = null;
        _productRepository
            .Setup(r => r.Add(It.IsAny<Product>()))
            .Callback<Product>(product => addedProduct = product);

        var result = await _handler.Handle(_command, _token);

        Assert.True(result.IsSuccess);
        Assert.NotNull(addedProduct);
        Assert.Equal(ValidName, addedProduct!.Name.Value);
        Assert.Equal(ValidPhone, addedProduct.ManufacturePhone.Value);
        Assert.Equal("test@example.com", addedProduct.ManufactureEmail.Value);
        Assert.True(addedProduct.IsAvailable);
        Assert.Equal(addedProduct.Id, result.Value.Id);
        Assert.Equal(ValidName, result.Value.Name);
        _productRepository.Verify(r => r.GetByNameAsync(_command.Name, _token), Times.Once);
        _productRepository.Verify(r => r.Add(It.IsAny<Product>()), Times.Once);
        _productRepository.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task Handle_WhenUserIsNotAuthenticated_ReturnsUnauthorizedAndDoesNotAddProduct()
    {
        _currentUserService
            .Setup(s => s.GetCurrentUserAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync((CurrentUser?)null);

        var result = await _handler.Handle(_command, _token);

        Assert.True(result.IsFailure);
        Assert.Equal(ErrorType.Unauthorized, result.Error.ErrorType);
        _productRepository.Verify(r => r.GetByNameAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Never);
        _productRepository.Verify(r => r.Add(It.IsAny<Product>()), Times.Never);
    }

    [Fact]
    public async Task Handle_WhenProductNameAlreadyExists_ReturnsConflictAndDoesNotAddProduct()
    {
        var existingProduct = Product.Create(ValidName, ValidPhone, ValidEmail, isAvailable: true);
        _productRepository
            .Setup(r => r.GetByNameAsync(_command.Name, _token))
            .ReturnsAsync(existingProduct);

        var result = await _handler.Handle(_command, _token);

        Assert.True(result.IsFailure);
        Assert.Equal(ErrorType.Conflict, result.Error.ErrorType);
        _productRepository.Verify(r => r.Add(It.IsAny<Product>()), Times.Never);
    }

    [Fact]
    public async Task Handle_WhenUserIsNotAuthenticated_DoesNotExposeValue()
    {
        _currentUserService
            .Setup(s => s.GetCurrentUserAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync((CurrentUser?)null);

        var result = await _handler.Handle(_command, _token);

        Assert.Throws<InvalidOperationException>(() => _ = result.Value);
    }
}
