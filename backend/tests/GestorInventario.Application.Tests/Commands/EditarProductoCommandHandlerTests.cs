using FluentAssertions;
using GestorInventario.Application.Abstractions;
using GestorInventario.Application.Exceptions;
using GestorInventario.Application.Productos.Commands.EditarProducto;
using GestorInventario.Domain.Entities;
using NSubstitute;
using Xunit;

namespace GestorInventario.Application.Tests.Commands;

public class EditarProductoCommandHandlerTests
{
    private readonly IProductoRepository _productoRepository = Substitute.For<IProductoRepository>();
    private readonly ICategoriaRepository _categoriaRepository = Substitute.For<ICategoriaRepository>();
    private readonly IUnitOfWork _unitOfWork = Substitute.For<IUnitOfWork>();
    private readonly EditarProductoCommandHandler _handler;

    public EditarProductoCommandHandlerTests()
    {
        _handler = new EditarProductoCommandHandler(_productoRepository, _categoriaRepository, _unitOfWork);
    }

    [Fact]
    public async Task Handle_WhenProductExistsAndCategoryValid_ShouldUpdateAndReturnDto()
    {
        // Arrange
        var producto = Producto.Create("Teclado", "USB", 50000m, 10, 5, 1);
        _productoRepository.GetByIdAsync(1, Arg.Any<CancellationToken>()).Returns(producto);
        _categoriaRepository.ExistsAsync(2, Arg.Any<CancellationToken>()).Returns(true);
        _categoriaRepository.GetByIdAsync(2, Arg.Any<CancellationToken>()).Returns(Categoria.Create("Oficina"));

        var command = new EditarProductoCommand(1, "Teclado Ergonómico", "Inalámbrico", 90000m, 4, 2);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Nombre.Should().Be("Teclado Ergonómico");
        result.Precio.Should().Be(90000m);
        result.CategoriaId.Should().Be(2);
        await _unitOfWork.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_WhenProductDoesNotExist_ShouldThrowNotFoundException()
    {
        // Arrange
        _productoRepository.GetByIdAsync(99, Arg.Any<CancellationToken>()).Returns((Producto?)null);
        var command = new EditarProductoCommand(99, "Nombre", "Desc", 1000m, 5, 1);

        // Act
        Func<Task> act = () => _handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<NotFoundException>()
            .WithMessage("*no fue encontrado*");
    }

    [Fact]
    public async Task Handle_WhenCategoryDoesNotExist_ShouldThrowApplicationValidationException()
    {
        // Arrange
        var producto = Producto.Create("Teclado", "USB", 50000m, 10, 5, 1);
        _productoRepository.GetByIdAsync(1, Arg.Any<CancellationToken>()).Returns(producto);
        _categoriaRepository.ExistsAsync(999, Arg.Any<CancellationToken>()).Returns(false);

        var command = new EditarProductoCommand(1, "Teclado", "USB", 50000m, 5, 999);

        // Act
        Func<Task> act = () => _handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<ApplicationValidationException>()
            .WithMessage("*categoría*");
    }
}

