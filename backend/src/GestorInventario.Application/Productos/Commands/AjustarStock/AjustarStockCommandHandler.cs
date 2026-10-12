using GestorInventario.Application.Abstractions;
using GestorInventario.Application.Exceptions;
using GestorInventario.Application.Productos.Common;
using GestorInventario.Domain.Entities;
using MediatR;

namespace GestorInventario.Application.Productos.Commands.AjustarStock;

public class AjustarStockCommandHandler : IRequestHandler<AjustarStockCommand, ProductoDto>
{
    private readonly IProductoRepository _productoRepository;
    private readonly IMovimientoInventarioRepository _movimientoRepository;
    private readonly IUnitOfWork _unitOfWork;

    public AjustarStockCommandHandler(
        IProductoRepository productoRepository,
        IMovimientoInventarioRepository movimientoRepository,
        IUnitOfWork unitOfWork)
    {
        _productoRepository = productoRepository;
        _movimientoRepository = movimientoRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<ProductoDto> Handle(AjustarStockCommand request, CancellationToken cancellationToken)
    {
        var producto = await _productoRepository.GetByIdAsync(request.ProductoId, cancellationToken);
        if (producto is null)
        {
            throw new NotFoundException("Producto", request.ProductoId);
        }

        producto.AdjustStock(request.Tipo, request.Cantidad);

        var movimiento = new MovimientoInventario(
            request.ProductoId,
            request.Tipo,
            request.Cantidad,
            request.Observacion);

        await _movimientoRepository.AddAsync(movimiento, cancellationToken);
        _productoRepository.Update(producto);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return ProductoDto.FromDomain(producto);
    }
}
