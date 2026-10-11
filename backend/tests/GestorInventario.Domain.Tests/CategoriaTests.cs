using FluentAssertions;
using GestorInventario.Domain.Entities;
using GestorInventario.Domain.Exceptions;
using Xunit;

namespace GestorInventario.Domain.Tests;

public class CategoriaTests
{
    [Fact]
    public void Create_WithValidData_ShouldInstantiateCorrectly()
    {
        // Act
        var categoria = Categoria.Create("Electrónica");

        // Assert
        categoria.Nombre.Should().Be("Electrónica");
        categoria.Activo.Should().BeTrue();
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void Create_WithInvalidNombre_ShouldThrowDomainValidationException(string? nombre)
    {
        // Act
        Action act = () => Categoria.Create(nombre!);

        // Assert
        act.Should().Throw<DomainValidationException>()
            .WithMessage("*Nombre*");
    }

    [Fact]
    public void Create_WithNombreExceeding100Chars_ShouldThrowDomainValidationException()
    {
        // Arrange
        var longName = new string('A', 101);

        // Act
        Action act = () => Categoria.Create(longName);

        // Assert
        act.Should().Throw<DomainValidationException>()
            .WithMessage("*100*");
    }
}
