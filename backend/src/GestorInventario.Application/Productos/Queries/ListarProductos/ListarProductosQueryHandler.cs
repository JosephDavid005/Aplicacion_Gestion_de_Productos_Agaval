using GestorInventario.Application.Abstractions;
using GestorInventario.Application.Productos.Common;
using MediatR;

namespace GestorInventario.Application.Productos.Queries.ListarProductos;

public class ListarProductosQueryHandler : IRequestHandler<ListarProductosQuery, IReadOnlyList<ProductoDto>>
{
    private readonly IProductoRepository _productoRepository;

    public ListarProductosQueryHandler(IProductoRepository productoRepository)
    {
        _productoRepository = productoRepository;
    }

    public async Task<IReadOnlyList<ProductoDto>> Handle(ListarProductosQuery request, CancellationToken cancellationToken)
    {
        var productos = await _productoRepository.GetAllAsync(cancellationToken);
        return productos.Select(p => ProductoDto.FromDomain(p)).ToList();
    }
}
