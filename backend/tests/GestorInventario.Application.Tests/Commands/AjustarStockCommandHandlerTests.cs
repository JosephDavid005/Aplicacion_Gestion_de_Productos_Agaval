using FluentAssertions;
using GestorInventario.Application.Abstractions;
using GestorInventario.Application.Exceptions;
using GestorInventario.Application.Productos.Commands.AjustarStock;
using GestorInventario.Domain.Entities;
using GestorInventario.Domain.Enums;
using GestorInventario.Domain.Exceptions;
using NSubstitute;
using Xunit;

namespace GestorInventario.Application.Tests.Commands;

public class AjustarStockCommandHandlerTests
{
    private readonly IProductoRepository _productoRepository = Substitute.For<IProductoRepository>();
    private readonly IUnitOfWork _unitOfWork = Substitute.For<IUnitOfWork>();
    private readonly AjustarStockCommandHandler _handler;

    public AjustarStockCommandHandlerTests()
    {
        _handler = new AjustarStockCommandHandler(_productoRepository, _unitOfWork);
    }

    [Fact]
    public async Task Handle_WithValidEntrada_ShouldIncreaseStockAndSave()
    {
        // Arrange
        var producto = Producto.Create("Impresora", "Multifuncional", 800000m, stock: 5, stockMinimo: 2, categoriaId: 2);
        _productoRepository.GetByIdAsync(1, Arg.Any<CancellationToken>()).Returns(producto);

        var command = new AjustarStockCommand(1, TipoMovimiento.Entrada, 3);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Stock.Should().Be(8);
        await _unitOfWork.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_WithValidSalida_ShouldDecreaseStockAndSave()
    {
        // Arrange
        var producto = Producto.Create("Impresora", "Multifuncional", 800000m, stock: 5, stockMinimo: 2, categoriaId: 2);
        _productoRepository.GetByIdAsync(1, Arg.Any<CancellationToken>()).Returns(producto);

        var command = new AjustarStockCommand(1, TipoMovimiento.Salida, 4);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Stock.Should().Be(1);
        result.IsLowStock.Should().BeTrue();
        await _unitOfWork.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_WhenProductNotFound_ShouldThrowNotFoundException()
    {
        // Arrange
        _productoRepository.GetByIdAsync(99, Arg.Any<CancellationToken>()).Returns((Producto?)null);
        var command = new AjustarStockCommand(99, TipoMovimiento.Entrada, 5);

        // Act
        Func<Task> act = () => _handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<NotFoundException>()
            .WithMessage("*no fue encontrado*");
        await _unitOfWork.DidNotReceive().SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_WhenSalidaExceedsStock_ShouldPropagateDomainValidationExceptionAndNotSave()
    {
        // Arrange
        var producto = Producto.Create("Impresora", "Multifuncional", 800000m, stock: 2, stockMinimo: 1, categoriaId: 2);
        _productoRepository.GetByIdAsync(1, Arg.Any<CancellationToken>()).Returns(producto);

        var command = new AjustarStockCommand(1, TipoMovimiento.Salida, 5);

        // Act
        Func<Task> act = () => _handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<DomainValidationException>()
            .WithMessage("*insuficiente*");
        producto.Stock.Should().Be(2);
        await _unitOfWork.DidNotReceive().SaveChangesAsync(Arg.Any<CancellationToken>());
    }
}
