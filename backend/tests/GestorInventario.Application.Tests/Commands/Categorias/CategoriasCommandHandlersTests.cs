using FluentAssertions;
using GestorInventario.Application.Abstractions;
using GestorInventario.Application.Categorias.Commands.CrearCategoria;
using GestorInventario.Application.Categorias.Commands.EditarCategoria;
using GestorInventario.Application.Categorias.Commands.EliminarCategoria;
using GestorInventario.Application.Exceptions;
using GestorInventario.Domain.Entities;
using GestorInventario.Domain.Exceptions;
using MediatR;
using NSubstitute;
using Xunit;

namespace GestorInventario.Application.Tests.Commands.Categorias;

public class CategoriasCommandHandlersTests
{
    private readonly ICategoriaRepository _categoriaRepository = Substitute.For<ICategoriaRepository>();
    private readonly IUnitOfWork _unitOfWork = Substitute.For<IUnitOfWork>();

    [Fact]
    public async Task CrearCategoria_WhenValid_ShouldCreateAndSave()
    {
        // Arrange
        _categoriaRepository.ExistsByNameAsync("Deportes", null, Arg.Any<CancellationToken>()).Returns(false);
        var handler = new CrearCategoriaCommandHandler(_categoriaRepository, _unitOfWork);
        var command = new CrearCategoriaCommand("Deportes", true);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.Nombre.Should().Be("Deportes");
        result.Activo.Should().BeTrue();
        await _categoriaRepository.Received(1).AddAsync(Arg.Is<Categoria>(c => c.Nombre == "Deportes"), Arg.Any<CancellationToken>());
        await _unitOfWork.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task CrearCategoria_WhenNameAlreadyExists_ShouldThrowDomainValidationException()
    {
        // Arrange
        _categoriaRepository.ExistsByNameAsync("Electrónica", null, Arg.Any<CancellationToken>()).Returns(true);
        var handler = new CrearCategoriaCommandHandler(_categoriaRepository, _unitOfWork);
        var command = new CrearCategoriaCommand("Electrónica", true);

        // Act
        Func<Task> act = () => handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<DomainValidationException>()
            .WithMessage("*Ya existe una categoría*");
        await _categoriaRepository.DidNotReceive().AddAsync(Arg.Any<Categoria>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task EditarCategoria_WhenValid_ShouldUpdateAndSave()
    {
        // Arrange
        var categoria = Categoria.Create("Hogar Antiguo", activo: true);
        _categoriaRepository.GetByIdAsync(3, Arg.Any<CancellationToken>()).Returns(categoria);
        _categoriaRepository.ExistsByNameAsync("Hogar Moderno", 3, Arg.Any<CancellationToken>()).Returns(false);

        var handler = new EditarCategoriaCommandHandler(_categoriaRepository, _unitOfWork);
        var command = new EditarCategoriaCommand(3, "Hogar Moderno", false);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.Nombre.Should().Be("Hogar Moderno");
        result.Activo.Should().BeFalse();
        _categoriaRepository.Received(1).Update(categoria);
        await _unitOfWork.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task EditarCategoria_WhenNotFound_ShouldThrowNotFoundException()
    {
        // Arrange
        _categoriaRepository.GetByIdAsync(99, Arg.Any<CancellationToken>()).Returns((Categoria?)null);
        var handler = new EditarCategoriaCommandHandler(_categoriaRepository, _unitOfWork);
        var command = new EditarCategoriaCommand(99, "Nueva Cat", true);

        // Act
        Func<Task> act = () => handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<NotFoundException>()
            .WithMessage("*no fue encontrado*");
    }

    [Fact]
    public async Task EliminarCategoria_WhenHasProducts_ShouldThrowInvalidOperationException()
    {
        // Arrange
        var categoria = Categoria.Create("Ropa", activo: true);
        _categoriaRepository.GetByIdAsync(2, Arg.Any<CancellationToken>()).Returns(categoria);
        _categoriaRepository.HasProductsAsync(2, Arg.Any<CancellationToken>()).Returns(true);

        var handler = new EliminarCategoriaCommandHandler(_categoriaRepository, _unitOfWork);
        var command = new EliminarCategoriaCommand(2);

        // Act
        Func<Task> act = () => handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("*productos asociados*");
        _categoriaRepository.DidNotReceive().Delete(Arg.Any<Categoria>());
    }

    [Fact]
    public async Task EliminarCategoria_WhenNoProducts_ShouldDeleteAndSave()
    {
        // Arrange
        var categoria = Categoria.Create("Sin Productos", activo: true);
        _categoriaRepository.GetByIdAsync(4, Arg.Any<CancellationToken>()).Returns(categoria);
        _categoriaRepository.HasProductsAsync(4, Arg.Any<CancellationToken>()).Returns(false);

        var handler = new EliminarCategoriaCommandHandler(_categoriaRepository, _unitOfWork);
        var command = new EliminarCategoriaCommand(4);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().Be(Unit.Value);
        _categoriaRepository.Received(1).Delete(categoria);
        await _unitOfWork.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }
}
