using GestorInventario.Application.Abstractions;
using GestorInventario.Application.Exceptions;
using MediatR;

namespace GestorInventario.Application.Productos.Commands.EliminarProducto;

public class EliminarProductoCommandHandler : IRequestHandler<EliminarProductoCommand, Unit>
{
    private readonly IProductoRepository _productoRepository;
    private readonly IUnitOfWork _unitOfWork;

    public EliminarProductoCommandHandler(
        IProductoRepository productoRepository,
        IUnitOfWork unitOfWork)
    {
        _productoRepository = productoRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Unit> Handle(EliminarProductoCommand request, CancellationToken cancellationToken)
    {
        var producto = await _productoRepository.GetByIdAsync(request.Id, cancellationToken);
        if (producto is null)
        {
            throw new NotFoundException("Producto", request.Id);
        }

        _productoRepository.Delete(producto);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }
}
