using GestorInventario.Domain.Entities;

namespace GestorInventario.Application.Productos.Common;

public record ProductoDto(
    int Id,
    string Nombre,
    string? Descripcion,
    decimal Precio,
    int Stock,
    int StockMinimo,
    int CategoriaId,
    string? CategoriaNombre,
    bool IsLowStock,
    DateTime FechaCreacion
)
{
    public static ProductoDto FromDomain(Producto p, string? categoriaNombre = null) =>
        new(
            p.Id,
            p.Nombre,
            p.Descripcion,
            p.Precio,
            p.Stock,
            p.StockMinimo,
            p.CategoriaId,
            categoriaNombre ?? p.Categoria?.Nombre,
            p.IsLowStock,
            p.FechaCreacion
        );
}
