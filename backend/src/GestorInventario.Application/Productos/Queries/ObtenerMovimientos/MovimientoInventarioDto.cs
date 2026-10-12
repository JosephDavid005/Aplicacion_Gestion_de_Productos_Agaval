using GestorInventario.Domain.Entities;

namespace GestorInventario.Application.Productos.Queries.ObtenerMovimientos;

public record MovimientoInventarioDto(
    int Id,
    int ProductoId,
    string TipoMovimiento,
    int Cantidad,
    DateTime Fecha,
    string? Observacion
)
{
    public static MovimientoInventarioDto FromDomain(MovimientoInventario m) =>
        new(
            m.Id,
            m.ProductoId,
            m.TipoMovimiento.ToString().ToUpperInvariant(),
            m.Cantidad,
            m.Fecha,
            m.Observacion);
}
