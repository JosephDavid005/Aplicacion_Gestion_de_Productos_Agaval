using FluentAssertions;
using GestorInventario.Application.Abstractions;
using GestorInventario.Application.Productos.Queries.ListarProductosStockBajo;
using GestorInventario.Domain.Entities;
using NSubstitute;
using Xunit;

namespace GestorInventario.Application.Tests.Queries;

public class ListarProductosStockBajoQueryHandlerTests
{
    private readonly IProductoRepository _productoRepository = Substitute.For<IProductoRepository>();
    private readonly ListarProductosStockBajoQueryHandler _handler;

    public ListarProductosStockBajoQueryHandlerTests()
    {
        _handler = new ListarProductosStockBajoQueryHandler(_productoRepository);
    }

    [Fact]
    public async Task Handle_ShouldReturnOnlyLowStockProducts()
    {
        // Arrange (Stock < StockMinimo)
        var lowStockProducts = new List<Producto>
        {
            Producto.Create("Tóner", "Negro", 80000m, stock: 1, stockMinimo: 3, categoriaId: 2),
            Producto.Create("Papel Carta", "500 hojas", 25000m, stock: 0, stockMinimo: 5, categoriaId: 2)
        };
        _productoRepository.GetLowStockAsync(Arg.Any<CancellationToken>()).Returns(lowStockProducts);

        var query = new ListarProductosStockBajoQuery();

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().HaveCount(2);
        result.Should().OnlyContain(p => p.IsLowStock);
    }
}
