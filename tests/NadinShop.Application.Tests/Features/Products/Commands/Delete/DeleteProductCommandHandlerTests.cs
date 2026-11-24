using Moq;
using NadinShop.Application.Common.Interfaces;
using NadinShop.Application.Common.ResultPattern;
using NadinShop.Application.Common.Services;
using NadinShop.Application.Features.Products.Commands.Delete;
using NadinShop.Domain.Entities.Products;

namespace NadinShop.Application.Tests.Features.Products.Commands.Delete;

public class DeleteProductCommandHandlerTests
{
    private const string ValidPhone = "09121234567";
    private const string ValidEmail = "Test@Example.com";

    private readonly Product _product;
    private readonly DeleteProductCommand _command;
    private readonly CancellationToken _token;
    private readonly CurrentUser _currentUser;
    private readonly Mock<IProductRepository> _productRepository;
    private readonly Mock<ICurrentUserService> _currentUserService;
    private readonly DeleteProductCommandHandler _handler;

    public DeleteProductCommandHandlerTests()
    {
        using var cancellationTokenSource = new CancellationTokenSource();
        _token = cancellationTokenSource.Token;
        _currentUser = new CurrentUser(Guid.NewGuid(), "test@example.com", ValidPhone);
        _product = Product.Create("Galaxy S24", ValidPhone, ValidEmail, isAvailable: true);
        _command = new DeleteProductCommand(_product.Id);

        _productRepository = new Mock<IProductRepository>();
        _currentUserService = new Mock<ICurrentUserService>();
        _currentUserService
            .Setup(s => s.GetCurrentUserAsync(_token))
            .ReturnsAsync(_currentUser);
        _productRepository
            .Setup(r => r.GetByIdAsync(_product.Id, _token))
            .ReturnsAsync(_product);

        _handler = new DeleteProductCommandHandler(_productRepository.Object, _currentUserService.Object);
    }

    [Fact]
    public async Task Handle_WithOwner_RemovesProductAndReturnsSuccess()
    {
        var result = await _handler.Handle(_command, _token);

        Assert.True(result.IsSuccess);
        _productRepository.Verify(r => r.GetByIdAsync(_product.Id, _token), Times.Once);
        _productRepository.Verify(r => r.Remove(_product), Times.Once);
        _productRepository.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task Handle_WhenUserIsNotAuthenticated_ReturnsUnauthorizedAndDoesNotRemoveProduct()
    {
        _currentUserService
            .Setup(s => s.GetCurrentUserAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync((CurrentUser?)null);

        var result = await _handler.Handle(_command, _token);

        Assert.True(result.IsFailure);
        Assert.Equal(ErrorType.Unauthorized, result.Error.ErrorType);
        _productRepository.Verify(r => r.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()), Times.Never);
        _productRepository.Verify(r => r.Remove(It.IsAny<Product>()), Times.Never);
    }

    [Fact]
    public async Task Handle_WhenProductDoesNotExist_ReturnsNotFoundAndDoesNotRemoveProduct()
    {
        _productRepository
            .Setup(r => r.GetByIdAsync(_product.Id, _token))
            .ReturnsAsync((Product?)null);

        var result = await _handler.Handle(_command, _token);

        Assert.True(result.IsFailure);
        Assert.Equal(ErrorType.NotFound, result.Error.ErrorType);
        _productRepository.Verify(r => r.Remove(It.IsAny<Product>()), Times.Never);
    }

    [Fact]
    public async Task Handle_WhenContactBelongsToAnotherUser_ReturnsForbiddenAndDoesNotRemoveProduct()
    {
        _currentUserService
            .Setup(s => s.GetCurrentUserAsync(_token))
            .ReturnsAsync(new CurrentUser(Guid.NewGuid(), "someone-else@example.com", "09351234567"));

        var result = await _handler.Handle(_command, _token);

        Assert.True(result.IsFailure);
        Assert.Equal(ErrorType.Forbidden, result.Error.ErrorType);
        _productRepository.Verify(r => r.Remove(It.IsAny<Product>()), Times.Never);
    }
}
