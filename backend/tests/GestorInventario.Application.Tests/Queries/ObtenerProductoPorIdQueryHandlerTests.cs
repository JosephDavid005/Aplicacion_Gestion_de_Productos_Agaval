using FluentAssertions;
using GestorInventario.Application.Abstractions;
using GestorInventario.Application.Exceptions;
using GestorInventario.Application.Productos.Queries.ObtenerProductoPorId;
using GestorInventario.Domain.Entities;
using NSubstitute;
using Xunit;

namespace GestorInventario.Application.Tests.Queries;

public class ObtenerProductoPorIdQueryHandlerTests
{
    private readonly IProductoRepository _productoRepository = Substitute.For<IProductoRepository>();
    private readonly ObtenerProductoPorIdQueryHandler _handler;

    public ObtenerProductoPorIdQueryHandlerTests()
    {
        _handler = new ObtenerProductoPorIdQueryHandler(_productoRepository);
    }

    [Fact]
    public async Task Handle_WhenProductExists_ShouldReturnDto()
    {
        // Arrange
        var producto = Producto.Create("Monitor", "4K", 1200000m, 6, 2, 1);
        _productoRepository.GetByIdAsync(1, Arg.Any<CancellationToken>()).Returns(producto);

        var query = new ObtenerProductoPorIdQuery(1);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Nombre.Should().Be("Monitor");
        result.Precio.Should().Be(1200000m);
    }

    [Fact]
    public async Task Handle_WhenProductDoesNotExist_ShouldThrowNotFoundException()
    {
        // Arrange
        _productoRepository.GetByIdAsync(999, Arg.Any<CancellationToken>()).Returns((Producto?)null);
        var query = new ObtenerProductoPorIdQuery(999);

        // Act
        Func<Task> act = () => _handler.Handle(query, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<NotFoundException>()
            .WithMessage("*no fue encontrado*");
    }
}
