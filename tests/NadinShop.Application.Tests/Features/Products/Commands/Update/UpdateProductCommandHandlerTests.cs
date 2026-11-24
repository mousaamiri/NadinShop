using Moq;
using NadinShop.Application.Common.Interfaces;
using NadinShop.Application.Common.ResultPattern;
using NadinShop.Application.Common.Services;
using NadinShop.Application.Features.Products.Commands.Update;
using NadinShop.Domain.Entities.Products;

namespace NadinShop.Application.Tests.Features.Products.Commands.Update;

public class UpdateProductCommandHandlerTests
{
    private const string OriginalName = "Galaxy S24";
    private const string OriginalPhone = "09121234567";
    private const string OriginalEmail = "Test@Example.com";
    private const string NewName = "iPhone 15 Pro";
    private const string NewPhone = "09351234567";
    private const string NewEmail = "Support@NadinShop.COM";

    private readonly Product _product;
    private readonly UpdateProductCommand _command;
    private readonly CancellationToken _token;
    private readonly CurrentUser _currentUser;
    private readonly Mock<IProductRepository> _productRepository;
    private readonly Mock<ICurrentUserService> _currentUserService;
    private readonly UpdateProductCommandHandler _handler;

    public UpdateProductCommandHandlerTests()
    {
        using var cancellationTokenSource = new CancellationTokenSource();
        _token = cancellationTokenSource.Token;
        _currentUser = new CurrentUser(Guid.NewGuid(), OriginalEmail, OriginalPhone);
        _product = Product.Create(OriginalName, OriginalPhone, OriginalEmail, isAvailable: true);
        _command = new UpdateProductCommand(_product.Id, NewName, IsAvailable: false);

        _productRepository = new Mock<IProductRepository>();
        _currentUserService = new Mock<ICurrentUserService>();
        _currentUserService
            .Setup(s => s.GetCurrentUserAsync(_token))
            .ReturnsAsync(_currentUser);
        _productRepository
            .Setup(r => r.GetByIdAsync(_product.Id, _token))
            .ReturnsAsync(_product);
        _productRepository
            .Setup(r => r.GetByNameAsync(_command.Name, _token))
            .ReturnsAsync((Product?)null);

        _handler = new UpdateProductCommandHandler(_productRepository.Object, _currentUserService.Object);
    }

    [Fact]
    public async Task Handle_WithOwnerAndValidValues_UpdatesProductAndReturnsUpdatedValues()
    {
        var result = await _handler.Handle(_command, _token);

        Assert.True(result.IsSuccess);
        Assert.Equal(NewName, _product.Name.Value);
        Assert.Equal(OriginalPhone, _product.ManufacturePhone.Value);
        Assert.Equal(OriginalEmail.ToLowerInvariant(), _product.ManufactureEmail.Value);
        Assert.False(_product.IsAvailable);
        Assert.Equal(_product.Id, result.Value.Id);
        Assert.Equal(NewName, result.Value.Name);
        Assert.False(result.Value.IsAvailable);
        _productRepository.Verify(r => r.GetByIdAsync(_product.Id, _token), Times.Once);
        _productRepository.Verify(r => r.GetByNameAsync(_command.Name, _token), Times.Once);
        _productRepository.Verify(r => r.Update(_product), Times.Once);
        _productRepository.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task Handle_WhenUserIsNotAuthenticated_ReturnsUnauthorizedAndDoesNotUpdateProduct()
    {
        _currentUserService
            .Setup(s => s.GetCurrentUserAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync((CurrentUser?)null);

        var result = await _handler.Handle(_command, _token);

        Assert.True(result.IsFailure);
        Assert.Equal(ErrorType.Unauthorized, result.Error.ErrorType);
        Assert.Equal(OriginalName, _product.Name.Value);
        _productRepository.Verify(r => r.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()), Times.Never);
        _productRepository.Verify(r => r.Update(It.IsAny<Product>()), Times.Never);
    }

    [Fact]
    public async Task Handle_WhenProductDoesNotExist_ReturnsNotFoundAndDoesNotUpdateProduct()
    {
        _productRepository
            .Setup(r => r.GetByIdAsync(_product.Id, _token))
            .ReturnsAsync((Product?)null);

        var result = await _handler.Handle(_command, _token);

        Assert.True(result.IsFailure);
        Assert.Equal(ErrorType.NotFound, result.Error.ErrorType);
        _productRepository.Verify(r => r.Update(It.IsAny<Product>()), Times.Never);
    }

    [Fact]
    public async Task Handle_WhenManufactureEmailBelongsToAnotherUser_ReturnsForbiddenAndKeepsProductUnchanged()
    {
        _currentUserService
            .Setup(s => s.GetCurrentUserAsync(_token))
            .ReturnsAsync(new CurrentUser(Guid.NewGuid(), "someone-else@example.com", OriginalPhone));

        var result = await _handler.Handle(_command, _token);

        Assert.True(result.IsFailure);
        Assert.Equal(ErrorType.Forbidden, result.Error.ErrorType);
        Assert.Equal(OriginalName, _product.Name.Value);
        Assert.Equal(OriginalPhone, _product.ManufacturePhone.Value);
        Assert.Equal("test@example.com", _product.ManufactureEmail.Value);
        _productRepository.Verify(r => r.Update(It.IsAny<Product>()), Times.Never);
    }

    [Fact]
    public async Task Handle_WhenManufacturePhoneBelongsToAnotherUser_ReturnsForbiddenAndKeepsProductUnchanged()
    {
        _currentUserService
            .Setup(s => s.GetCurrentUserAsync(_token))
            .ReturnsAsync(new CurrentUser(Guid.NewGuid(), "test@example.com", "09351234567"));

        var result = await _handler.Handle(_command, _token);

        Assert.True(result.IsFailure);
        Assert.Equal(ErrorType.Forbidden, result.Error.ErrorType);
        Assert.Equal(OriginalName, _product.Name.Value);
        _productRepository.Verify(r => r.Update(It.IsAny<Product>()), Times.Never);
    }

    [Fact]
    public async Task Handle_WhenNewNameBelongsToAnotherProduct_ReturnsConflictAndKeepsProductUnchanged()
    {
        var otherProduct = Product.Create(NewName, NewPhone, NewEmail, isAvailable: true);
        _productRepository
            .Setup(r => r.GetByNameAsync(_command.Name, _token))
            .ReturnsAsync(otherProduct);

        var result = await _handler.Handle(_command, _token);

        Assert.True(result.IsFailure);
        Assert.Equal(ErrorType.Conflict, result.Error.ErrorType);
        Assert.Equal(OriginalName, _product.Name.Value);
        _productRepository.Verify(r => r.Update(It.IsAny<Product>()), Times.Never);
    }
}
