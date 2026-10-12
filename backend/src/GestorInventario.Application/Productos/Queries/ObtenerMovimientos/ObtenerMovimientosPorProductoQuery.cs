using MediatR;

namespace GestorInventario.Application.Productos.Queries.ObtenerMovimientos;

public record ObtenerMovimientosPorProductoQuery(int ProductoId)
    : IRequest<IReadOnlyList<MovimientoInventarioDto>>;
