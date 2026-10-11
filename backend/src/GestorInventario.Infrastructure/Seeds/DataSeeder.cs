using GestorInventario.Domain.Entities;
using GestorInventario.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace GestorInventario.Infrastructure.Seeds;

public static class DataSeeder
{
    public static async Task SeedAsync(PersistenceContext context, CancellationToken cancellationToken = default)
    {
        // 1. Seed de Categorías (según script oficial de Agaval)
        if (!await context.Categorias.AnyAsync(cancellationToken))
        {
            var categorias = new[]
            {
                Categoria.Create("Electrónica"),
                Categoria.Create("Oficina"),
                Categoria.Create("Aseo")
            };

            await context.Categorias.AddRangeAsync(categorias, cancellationToken);
            await context.SaveChangesAsync(cancellationToken);
        }

        // 2. Seed de Productos de muestra para pruebas
        if (!await context.Productos.AnyAsync(cancellationToken))
        {
            var electronica = await context.Categorias.FirstAsync(c => c.Nombre == "Electrónica", cancellationToken);
            var oficina = await context.Categorias.FirstAsync(c => c.Nombre == "Oficina", cancellationToken);
            var aseo = await context.Categorias.FirstAsync(c => c.Nombre == "Aseo", cancellationToken);

            var productos = new[]
            {
                Producto.Create("Portátil Lenovo ThinkPad", "Intel Core i5, 16GB RAM, 512GB SSD", 3200000.00m, stock: 12, stockMinimo: 5, electronica.Id),
                Producto.Create("Mouse Inalámbrico Logitech", "Sensor óptico 1000 DPI USB", 65000.00m, stock: 3, stockMinimo: 5, electronica.Id), // Stock bajo
                Producto.Create("Resma Papel Carta Reprograf", "500 hojas 75g blancura 95%", 24500.00m, stock: 20, stockMinimo: 10, oficina.Id),
                Producto.Create("Bolígrafo Kilométrico Negro", "Caja x 12 unidades", 18000.00m, stock: 4, stockMinimo: 8, oficina.Id), // Stock bajo
                Producto.Create("Detergente Multiusos 1L", "Biodegradable con aroma cítrico", 14500.00m, stock: 15, stockMinimo: 6, aseo.Id),
                Producto.Create("Desinfectante en Spray", "Elimina 99.9% bacterias 400ml", 19900.00m, stock: 2, stockMinimo: 5, aseo.Id) // Stock bajo
            };

            await context.Productos.AddRangeAsync(productos, cancellationToken);
            await context.SaveChangesAsync(cancellationToken);
        }
    }
}
