using FluentAssertions;
using GestorInventario.Application.Abstractions;
using GestorInventario.Application.Productos.Queries.ListarProductos;
using GestorInventario.Domain.Entities;
using NSubstitute;
using Xunit;

namespace GestorInventario.Application.Tests.Queries;

public class ListarProductosQueryHandlerTests
{
    private readonly IProductoRepository _productoRepository = Substitute.For<IProductoRepository>();
    private readonly ListarProductosQueryHandler _handler;

    public ListarProductosQueryHandlerTests()
    {
        _handler = new ListarProductosQueryHandler(_productoRepository);
    }

    [Fact]
    public async Task Handle_ShouldReturnAllProductsMappedToDto()
    {
        // Arrange
        var productos = new List<Producto>
        {
            Producto.Create("Laptop", "Desc 1", 3000000m, 10, 5, 1),
            Producto.Create("Mouse", "Desc 2", 50000m, 2, 5, 1)
        };
        _productoRepository.GetAllAsync(Arg.Any<CancellationToken>()).Returns(productos);

        var query = new ListarProductosQuery();

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().HaveCount(2);
        result[0].Nombre.Should().Be("Laptop");
        result[1].Nombre.Should().Be("Mouse");
        result[1].IsLowStock.Should().BeTrue();
    }
}
