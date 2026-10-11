using GestorInventario.Application.Abstractions;
using GestorInventario.Application.Exceptions;
using GestorInventario.Application.Productos.Common;
using MediatR;

namespace GestorInventario.Application.Productos.Commands.EditarProducto;

public class EditarProductoCommandHandler : IRequestHandler<EditarProductoCommand, ProductoDto>
{
    private readonly IProductoRepository _productoRepository;
    private readonly ICategoriaRepository _categoriaRepository;
    private readonly IUnitOfWork _unitOfWork;

    public EditarProductoCommandHandler(
        IProductoRepository productoRepository,
        ICategoriaRepository categoriaRepository,
        IUnitOfWork unitOfWork)
    {
        _productoRepository = productoRepository;
        _categoriaRepository = categoriaRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<ProductoDto> Handle(EditarProductoCommand request, CancellationToken cancellationToken)
    {
        var producto = await _productoRepository.GetByIdAsync(request.Id, cancellationToken);
        if (producto is null)
        {
            throw new NotFoundException("Producto", request.Id);
        }

        var categoryExists = await _categoriaRepository.ExistsAsync(request.CategoriaId, cancellationToken);
        if (!categoryExists)
        {
            throw new ApplicationValidationException($"La categoría con ID {request.CategoriaId} no existe.");
        }

        producto.UpdateDetails(
            nombre: request.Nombre,
            descripcion: request.Descripcion,
            precio: request.Precio,
            stockMinimo: request.StockMinimo,
            categoriaId: request.CategoriaId
        );

        _productoRepository.Update(producto);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        var categoria = await _categoriaRepository.GetByIdAsync(request.CategoriaId, cancellationToken);

        return ProductoDto.FromDomain(producto, categoria?.Nombre);
    }
}
