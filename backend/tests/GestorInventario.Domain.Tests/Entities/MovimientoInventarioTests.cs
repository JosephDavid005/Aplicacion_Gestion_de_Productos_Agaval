using FluentAssertions;
using GestorInventario.Domain.Entities;
using GestorInventario.Domain.Enums;
using GestorInventario.Domain.Exceptions;
using Xunit;

namespace GestorInventario.Domain.Tests.Entities;

public class MovimientoInventarioTests
{
    [Fact]
    public void Create_WithValidData_ShouldCreateMovimiento()
    {
        // Arrange & Act
        var movimiento = new MovimientoInventario(10, TipoMovimiento.Entrada, 5, "Compra inicial");

        // Assert
        movimiento.ProductoId.Should().Be(10);
        movimiento.TipoMovimiento.Should().Be(TipoMovimiento.Entrada);
        movimiento.Cantidad.Should().Be(5);
        movimiento.Observacion.Should().Be("Compra inicial");
        movimiento.Fecha.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    [InlineData(-50)]
    public void Create_WithInvalidCantidad_ShouldThrowDomainValidationException(int cantidadInvalida)
    {
        // Act
        Action act = () => new MovimientoInventario(10, TipoMovimiento.Entrada, cantidadInvalida);

        // Assert
        act.Should().Throw<DomainValidationException>()
            .WithMessage("*cantidad*mayor a cero*");
    }

    [Fact]
    public void Create_WithInvalidProductoId_ShouldThrowDomainValidationException()
    {
        // Act
        Action act = () => new MovimientoInventario(0, TipoMovimiento.Salida, 5);

        // Assert
        act.Should().Throw<DomainValidationException>()
            .WithMessage("*producto*válido*");
    }
}
