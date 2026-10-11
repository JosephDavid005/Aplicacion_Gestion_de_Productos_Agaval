using GestorInventario.Application.Abstractions;
using GestorInventario.Application.Productos.Common;
using MediatR;

namespace GestorInventario.Application.Productos.Queries.ListarProductosStockBajo;

public class ListarProductosStockBajoQueryHandler : IRequestHandler<ListarProductosStockBajoQuery, IReadOnlyList<ProductoDto>>
{
    private readonly IProductoRepository _productoRepository;

    public ListarProductosStockBajoQueryHandler(IProductoRepository productoRepository)
    {
        _productoRepository = productoRepository;
    }

    public async Task<IReadOnlyList<ProductoDto>> Handle(ListarProductosStockBajoQuery request, CancellationToken cancellationToken)
    {
        var productos = await _productoRepository.GetLowStockAsync(cancellationToken);
        return productos.Select(p => ProductoDto.FromDomain(p)).ToList();
    }
}
