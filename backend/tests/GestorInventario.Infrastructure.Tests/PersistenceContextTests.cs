using FluentAssertions;
using GestorInventario.Domain.Entities;
using GestorInventario.Infrastructure.Persistence;
using GestorInventario.Infrastructure.Repositories;
using GestorInventario.Infrastructure.Seeds;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace GestorInventario.Infrastructure.Tests;

public class PersistenceContextTests : IDisposable
{
    private readonly SqliteConnection _connection;
    private readonly PersistenceContext _context;

    public PersistenceContextTests()
    {
        _connection = new SqliteConnection("DataSource=:memory:");
        _connection.Open();

        var options = new DbContextOptionsBuilder<PersistenceContext>()
            .UseSqlite(_connection)
            .Options;

        _context = new PersistenceContext(options);
        _context.Database.EnsureCreated();
    }

    public void Dispose()
    {
        _context.Dispose();
        _connection.Dispose();
    }

    [Fact]
    public async Task DataSeeder_ShouldSeedInitialCategoriesAndProducts()
    {
        // Act
        await DataSeeder.SeedAsync(_context);

        // Assert
        var categorias = await _context.Categorias.ToListAsync();
        categorias.Should().HaveCount(3);
        categorias.Select(c => c.Nombre).Should().Contain(new[] { "Electrónica", "Oficina", "Aseo" });

        var productos = await _context.Productos.ToListAsync();
        productos.Should().NotBeEmpty();
    }

    [Fact]
    public async Task ProductoRepository_GetLowStockAsync_ShouldOnlyReturnProductsWhereStockLessThanStockMinimo()
    {
        // Arrange
        var cat = Categoria.Create("Tecnología");
        _context.Categorias.Add(cat);
        await _context.SaveChangesAsync();

        var prodNormal = Producto.Create("Prod 1", "Desc", 100m, stock: 5, stockMinimo: 5, categoriaId: cat.Id);
        var prodAlto = Producto.Create("Prod 2", "Desc", 100m, stock: 10, stockMinimo: 5, categoriaId: cat.Id);
        var prodBajo = Producto.Create("Prod 3", "Desc", 100m, stock: 4, stockMinimo: 5, categoriaId: cat.Id);

        _context.Productos.AddRange(prodNormal, prodAlto, prodBajo);
        await _context.SaveChangesAsync();

        var repo = new ProductoRepository(_context);

        // Act
        var lowStock = await repo.GetLowStockAsync();

        // Assert
        lowStock.Should().HaveCount(1);
        lowStock[0].Nombre.Should().Be("Prod 3");
    }

    [Fact]
    public async Task ProductoRepository_AddAndGetById_ShouldPersistAndRetrieveWithCategory()
    {
        // Arrange
        var cat = Categoria.Create("Hogar");
        _context.Categorias.Add(cat);
        await _context.SaveChangesAsync();

        var prod = Producto.Create("Lámpara", "LED", 45000m, stock: 8, stockMinimo: 3, categoriaId: cat.Id);
        var repo = new ProductoRepository(_context);

        // Act
        await repo.AddAsync(prod);
        await _context.SaveChangesAsync();

        var retrieved = await repo.GetByIdAsync(prod.Id);

        // Assert
        retrieved.Should().NotBeNull();
        retrieved!.Nombre.Should().Be("Lámpara");
        retrieved.Categoria.Should().NotBeNull();
        retrieved.Categoria!.Nombre.Should().Be("Hogar");
    }

    [Fact]
    public async Task CategoriaRepository_GetAllActiveAsync_ShouldOnlyReturnActiveCategories()
    {
        // Arrange
        var cat1 = Categoria.Create("Activa 1", activo: true);
        var cat2 = Categoria.Create("Inactiva", activo: false);
        _context.Categorias.AddRange(cat1, cat2);
        await _context.SaveChangesAsync();

        var repo = new CategoriaRepository(_context);

        // Act
        var active = await repo.GetAllActiveAsync();

        // Assert
        active.Should().Contain(c => c.Nombre == "Activa 1");
        active.Should().NotContain(c => c.Nombre == "Inactiva");
    }
}
