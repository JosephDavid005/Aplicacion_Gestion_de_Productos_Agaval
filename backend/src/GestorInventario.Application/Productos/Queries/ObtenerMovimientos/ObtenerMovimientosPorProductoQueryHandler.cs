using GestorInventario.Application.Abstractions;
using GestorInventario.Application.Exceptions;
using MediatR;

namespace GestorInventario.Application.Productos.Queries.ObtenerMovimientos;

public class ObtenerMovimientosPorProductoQueryHandler
    : IRequestHandler<ObtenerMovimientosPorProductoQuery, IReadOnlyList<MovimientoInventarioDto>>
{
    private readonly IProductoRepository _productoRepository;
    private readonly IMovimientoInventarioRepository _movimientoRepository;

    public ObtenerMovimientosPorProductoQueryHandler(
        IProductoRepository productoRepository,
        IMovimientoInventarioRepository movimientoRepository)
    {
        _productoRepository = productoRepository;
        _movimientoRepository = movimientoRepository;
    }

    public async Task<IReadOnlyList<MovimientoInventarioDto>> Handle(
        ObtenerMovimientosPorProductoQuery request,
        CancellationToken cancellationToken)
    {
        var producto = await _productoRepository.GetByIdAsync(request.ProductoId, cancellationToken);
        if (producto is null)
        {
            throw new NotFoundException("Producto", request.ProductoId);
        }

        var movimientos = await _movimientoRepository.GetByProductoIdAsync(request.ProductoId, cancellationToken);
        return movimientos.Select(MovimientoInventarioDto.FromDomain).ToList();
    }
}
