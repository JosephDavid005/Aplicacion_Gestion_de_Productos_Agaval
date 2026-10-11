using GestorInventario.Application.Abstractions;
using GestorInventario.Application.Exceptions;
using GestorInventario.Application.Productos.Common;
using GestorInventario.Domain.Entities;
using MediatR;

namespace GestorInventario.Application.Productos.Commands.CrearProducto;

public class CrearProductoCommandHandler : IRequestHandler<CrearProductoCommand, ProductoDto>
{
    private readonly IProductoRepository _productoRepository;
    private readonly ICategoriaRepository _categoriaRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CrearProductoCommandHandler(
        IProductoRepository productoRepository,
        ICategoriaRepository categoriaRepository,
        IUnitOfWork unitOfWork)
    {
        _productoRepository = productoRepository;
        _categoriaRepository = categoriaRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<ProductoDto> Handle(CrearProductoCommand request, CancellationToken cancellationToken)
    {
        var categoryExists = await _categoriaRepository.ExistsAsync(request.CategoriaId, cancellationToken);
        if (!categoryExists)
        {
            throw new ApplicationValidationException($"La categoría con ID {request.CategoriaId} no existe.");
        }

        var producto = Producto.Create(
            nombre: request.Nombre,
            descripcion: request.Descripcion,
            precio: request.Precio,
            stock: request.Stock,
            stockMinimo: request.StockMinimo,
            categoriaId: request.CategoriaId
        );

        await _productoRepository.AddAsync(producto, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        var categoria = await _categoriaRepository.GetByIdAsync(request.CategoriaId, cancellationToken);

        return ProductoDto.FromDomain(producto, categoria?.Nombre);
    }
}
