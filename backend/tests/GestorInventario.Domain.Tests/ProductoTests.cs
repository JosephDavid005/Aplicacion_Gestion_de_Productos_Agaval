using FluentAssertions;
using GestorInventario.Domain.Entities;
using GestorInventario.Domain.Enums;
using GestorInventario.Domain.Exceptions;
using Xunit;

namespace GestorInventario.Domain.Tests;

public class ProductoTests
{
    [Fact]
    public void Create_WithValidData_ShouldInstantiateCorrectly()
    {
        // Act
        var producto = Producto.Create(
            nombre: "Laptop Dell",
            descripcion: "Core i7 16GB RAM",
            precio: 3500000.50m,
            stock: 10,
            stockMinimo: 5,
            categoriaId: 1
        );

        // Assert
        producto.Nombre.Should().Be("Laptop Dell");
        producto.Descripcion.Should().Be("Core i7 16GB RAM");
        producto.Precio.Should().Be(3500000.50m);
        producto.Stock.Should().Be(10);
        producto.StockMinimo.Should().Be(5);
        producto.CategoriaId.Should().Be(1);
        producto.IsLowStock.Should().BeFalse();
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void Create_WithInvalidNombre_ShouldThrowDomainValidationException(string? nombre)
    {
        Action act = () => Producto.Create(nombre!, "Desc", 100m, 5, 2, 1);
        act.Should().Throw<DomainValidationException>().WithMessage("*Nombre*");
    }

    [Fact]
    public void Create_WithNombreExceeding150Chars_ShouldThrowDomainValidationException()
    {
        var longName = new string('A', 151);
        Action act = () => Producto.Create(longName, "Desc", 100m, 5, 2, 1);
        act.Should().Throw<DomainValidationException>().WithMessage("*150*");
    }

    [Fact]
    public void Create_WithDescripcionExceeding500Chars_ShouldThrowDomainValidationException()
    {
        var longDesc = new string('D', 501);
        Action act = () => Producto.Create("Producto", longDesc, 100m, 5, 2, 1);
        act.Should().Throw<DomainValidationException>().WithMessage("*500*");
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-10)]
    [InlineData(-0.01)]
    public void Create_WithPriceLessThanOrEqualToZero_ShouldThrowDomainValidationException(decimal precio)
    {
        Action act = () => Producto.Create("Producto", "Desc", precio, 5, 2, 1);
        act.Should().Throw<DomainValidationException>().WithMessage("*Precio*");
    }

    [Fact]
    public void Create_WithNegativeStock_ShouldThrowDomainValidationException()
    {
        Action act = () => Producto.Create("Producto", "Desc", 100m, -1, 2, 1);
        act.Should().Throw<DomainValidationException>().WithMessage("*Stock*");
    }

    [Fact]
    public void Create_WithNegativeStockMinimo_ShouldThrowDomainValidationException()
    {
        Action act = () => Producto.Create("Producto", "Desc", 100m, 5, -1, 1);
        act.Should().Throw<DomainValidationException>().WithMessage("*StockMinimo*");
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void Create_WithInvalidCategoriaId_ShouldThrowDomainValidationException(int categoriaId)
    {
        Action act = () => Producto.Create("Producto", "Desc", 100m, 5, 2, categoriaId);
        act.Should().Throw<DomainValidationException>().WithMessage("*Categoria*");
    }

    [Fact]
    public void IsLowStock_WhenStockIsLessThanStockMinimo_ShouldBeTrue()
    {
        var producto = Producto.Create("Mouse", "Óptico", 50000m, stock: 3, stockMinimo: 5, categoriaId: 1);
        producto.IsLowStock.Should().BeTrue();
    }

    [Fact]
    public void IsLowStock_WhenStockEqualsStockMinimo_ShouldBeFalse()
    {
        // Regla explícita del estándar: Stock bajo es estrictamente Stock < StockMinimo
        var producto = Producto.Create("Mouse", "Óptico", 50000m, stock: 5, stockMinimo: 5, categoriaId: 1);
        producto.IsLowStock.Should().BeFalse();
    }

    [Fact]
    public void IsLowStock_WhenStockIsGreaterThanStockMinimo_ShouldBeFalse()
    {
        var producto = Producto.Create("Mouse", "Óptico", 50000m, stock: 8, stockMinimo: 5, categoriaId: 1);
        producto.IsLowStock.Should().BeFalse();
    }

    [Fact]
    public void AdjustStock_WithEntrada_ShouldIncreaseStock()
    {
        var producto = Producto.Create("Teclado", "Mecánico", 150000m, stock: 10, stockMinimo: 5, categoriaId: 1);

        producto.AdjustStock(TipoMovimiento.Entrada, 5);

        producto.Stock.Should().Be(15);
    }

    [Fact]
    public void AdjustStock_WithSalida_ShouldDecreaseStock()
    {
        var producto = Producto.Create("Teclado", "Mecánico", 150000m, stock: 10, stockMinimo: 5, categoriaId: 1);

        producto.AdjustStock(TipoMovimiento.Salida, 4);

        producto.Stock.Should().Be(6);
    }

    [Fact]
    public void AdjustStock_WithSalidaMatchingCurrentStock_ShouldLeaveStockAtZero()
    {
        var producto = Producto.Create("Teclado", "Mecánico", 150000m, stock: 10, stockMinimo: 5, categoriaId: 1);

        producto.AdjustStock(TipoMovimiento.Salida, 10);

        producto.Stock.Should().Be(0);
        producto.IsLowStock.Should().BeTrue();
    }

    [Fact]
    public void AdjustStock_WithSalidaGreaterThanStock_ShouldThrowAndNotModifyStock()
    {
        var producto = Producto.Create("Teclado", "Mecánico", 150000m, stock: 5, stockMinimo: 2, categoriaId: 1);

        Action act = () => producto.AdjustStock(TipoMovimiento.Salida, 6);

        act.Should().Throw<DomainValidationException>()
            .WithMessage("*insuficiente*");
        producto.Stock.Should().Be(5);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-5)]
    public void AdjustStock_WithZeroOrNegativeQuantity_ShouldThrowDomainValidationException(int cantidad)
    {
        var producto = Producto.Create("Teclado", "Mecánico", 150000m, stock: 10, stockMinimo: 5, categoriaId: 1);

        Action act = () => producto.AdjustStock(TipoMovimiento.Entrada, cantidad);

        act.Should().Throw<DomainValidationException>()
            .WithMessage("*cantidad*");
    }
}
