using FluentAssertions;
using GestorInventario.Application.Abstractions;
using GestorInventario.Application.Exceptions;
using GestorInventario.Application.Productos.Commands.EliminarProducto;
using GestorInventario.Domain.Entities;
using NSubstitute;
using Xunit;

namespace GestorInventario.Application.Tests.Commands;

public class EliminarProductoCommandHandlerTests
{
    private readonly IProductoRepository _productoRepository = Substitute.For<IProductoRepository>();
    private readonly IUnitOfWork _unitOfWork = Substitute.For<IUnitOfWork>();
    private readonly EliminarProductoCommandHandler _handler;

    public EliminarProductoCommandHandlerTests()
    {
        _handler = new EliminarProductoCommandHandler(_productoRepository, _unitOfWork);
    }

    [Fact]
    public async Task Handle_WhenProductExists_ShouldDeleteAndSaveChanges()
    {
        // Arrange
        var producto = Producto.Create("Mouse", "Inalámbrico", 45000m, 10, 5, 1);
        _productoRepository.GetByIdAsync(1, Arg.Any<CancellationToken>()).Returns(producto);

        var command = new EliminarProductoCommand(1);

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert
        _productoRepository.Received(1).Delete(producto);
        await _unitOfWork.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_WhenProductDoesNotExist_ShouldThrowNotFoundException()
    {
        // Arrange
        _productoRepository.GetByIdAsync(99, Arg.Any<CancellationToken>()).Returns((Producto?)null);
        var command = new EliminarProductoCommand(99);

        // Act
        Func<Task> act = () => _handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<NotFoundException>()
            .WithMessage("*no fue encontrado*");
        _productoRepository.DidNotReceive().Delete(Arg.Any<Producto>());
        await _unitOfWork.DidNotReceive().SaveChangesAsync(Arg.Any<CancellationToken>());
    }
}

