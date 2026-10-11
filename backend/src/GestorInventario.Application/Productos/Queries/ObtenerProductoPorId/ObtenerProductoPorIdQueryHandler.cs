using GestorInventario.Application.Abstractions;
using GestorInventario.Application.Exceptions;
using GestorInventario.Application.Productos.Common;
using MediatR;

namespace GestorInventario.Application.Productos.Queries.ObtenerProductoPorId;

public class ObtenerProductoPorIdQueryHandler : IRequestHandler<ObtenerProductoPorIdQuery, ProductoDto>
{
    private readonly IProductoRepository _productoRepository;

    public ObtenerProductoPorIdQueryHandler(IProductoRepository productoRepository)
    {
        _productoRepository = productoRepository;
    }

    public async Task<ProductoDto> Handle(ObtenerProductoPorIdQuery request, CancellationToken cancellationToken)
    {
        var producto = await _productoRepository.GetByIdAsync(request.Id, cancellationToken);
        if (producto is null)
        {
            throw new NotFoundException("Producto", request.Id);
        }

        return ProductoDto.FromDomain(producto);
    }
}
