using FluentAssertions;
using GestorInventario.Application.Abstractions;
using GestorInventario.Application.Exceptions;
using GestorInventario.Application.Productos.Commands.CrearProducto;
using GestorInventario.Domain.Entities;
using GestorInventario.Domain.Exceptions;
using NSubstitute;
using Xunit;

namespace GestorInventario.Application.Tests.Commands;

public class CrearProductoCommandHandlerTests
{
    private readonly IProductoRepository _productoRepository = Substitute.For<IProductoRepository>();
    private readonly ICategoriaRepository _categoriaRepository = Substitute.For<ICategoriaRepository>();
    private readonly IUnitOfWork _unitOfWork = Substitute.For<IUnitOfWork>();
    private readonly CrearProductoCommandHandler _handler;

    public CrearProductoCommandHandlerTests()
    {
        _handler = new CrearProductoCommandHandler(_productoRepository, _categoriaRepository, _unitOfWork);
    }

    [Fact]
    public async Task Handle_WhenValidCommandAndExistingCategory_ShouldCreateAndReturnDto()
    {
        // Arrange
        var command = new CrearProductoCommand("Monitor 24", "Full HD IPS", 650000m, 8, 3, 1);
        _categoriaRepository.ExistsAsync(1, Arg.Any<CancellationToken>()).Returns(true);
        _categoriaRepository.GetByIdAsync(1, Arg.Any<CancellationToken>()).Returns(Categoria.Create("Electrónica"));

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Nombre.Should().Be("Monitor 24");
        result.Precio.Should().Be(650000m);
        result.Stock.Should().Be(8);
        result.IsLowStock.Should().BeFalse();
        await _productoRepository.Received(1).AddAsync(Arg.Any<Producto>(), Arg.Any<CancellationToken>());
        await _unitOfWork.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_WhenCategoryDoesNotExist_ShouldThrowApplicationValidationException()
    {
        // Arrange
        var command = new CrearProductoCommand("Monitor 24", "Full HD IPS", 650000m, 8, 3, 99);
        _categoriaRepository.ExistsAsync(99, Arg.Any<CancellationToken>()).Returns(false);

        // Act
        Func<Task> act = () => _handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<ApplicationValidationException>()
            .WithMessage("*categoría*");
        await _productoRepository.DidNotReceive().AddAsync(Arg.Any<Producto>(), Arg.Any<CancellationToken>());
        await _unitOfWork.DidNotReceive().SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_WhenDomainInvariantsFail_ShouldPropagateDomainValidationException()
    {
        // Arrange
        var command = new CrearProductoCommand("", "Desc", -10m, -1, 5, 1);
        _categoriaRepository.ExistsAsync(1, Arg.Any<CancellationToken>()).Returns(true);

        // Act
        Func<Task> act = () => _handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<DomainValidationException>();
    }
}
