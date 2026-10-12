using FluentAssertions;
using GestorInventario.Application.Abstractions;
using GestorInventario.Application.Exceptions;
using GestorInventario.Application.Productos.Queries.ObtenerMovimientos;
using GestorInventario.Domain.Entities;
using GestorInventario.Domain.Enums;
using NSubstitute;
using Xunit;

namespace GestorInventario.Application.Tests.Queries;

public class ObtenerMovimientosPorProductoQueryHandlerTests
{
    private readonly IProductoRepository _productoRepository = Substitute.For<IProductoRepository>();
    private readonly IMovimientoInventarioRepository _movimientoRepository = Substitute.For<IMovimientoInventarioRepository>();
    private readonly ObtenerMovimientosPorProductoQueryHandler _handler;

    public ObtenerMovimientosPorProductoQueryHandlerTests()
    {
        _handler = new ObtenerMovimientosPorProductoQueryHandler(_productoRepository, _movimientoRepository);
    }

    [Fact]
    public async Task Handle_WhenProductExists_ShouldReturnMappedMovimientosList()
    {
        // Arrange
        var producto = Producto.Create("Mesa", "Madera", 300000m, stock: 10, stockMinimo: 2, categoriaId: 1);
        _productoRepository.GetByIdAsync(5, Arg.Any<CancellationToken>()).Returns(producto);

        var mov1 = new MovimientoInventario(5, TipoMovimiento.Entrada, 10, "Compra inicial");
        var mov2 = new MovimientoInventario(5, TipoMovimiento.Salida, 3, "Venta factura #101");
        IReadOnlyList<MovimientoInventario> movimientos = new List<MovimientoInventario> { mov2, mov1 };

        _movimientoRepository.GetByProductoIdAsync(5, Arg.Any<CancellationToken>()).Returns(movimientos);

        var query = new ObtenerMovimientosPorProductoQuery(5);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().HaveCount(2);
        result[0].ProductoId.Should().Be(5);
        result[0].TipoMovimiento.Should().Be("SALIDA");
        result[0].Cantidad.Should().Be(3);
        result[0].Observacion.Should().Be("Venta factura #101");

        result[1].TipoMovimiento.Should().Be("ENTRADA");
        result[1].Cantidad.Should().Be(10);
    }

    [Fact]
    public async Task Handle_WhenProductNotFound_ShouldThrowNotFoundException()
    {
        // Arrange
        _productoRepository.GetByIdAsync(99, Arg.Any<CancellationToken>()).Returns((Producto?)null);
        var query = new ObtenerMovimientosPorProductoQuery(99);

        // Act
        Func<Task> act = () => _handler.Handle(query, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<NotFoundException>()
            .WithMessage("*no fue encontrado*");
        await _movimientoRepository.DidNotReceive().GetByProductoIdAsync(Arg.Any<int>(), Arg.Any<CancellationToken>());
    }
}
