using GestorInventario.Application.Productos.Common;
using MediatR;

namespace GestorInventario.Application.Productos.Commands.EditarProducto;

public record EditarProductoCommand(
    int Id,
    string Nombre,
    string? Descripcion,
    decimal Precio,
    int StockMinimo,
    int CategoriaId
) : IRequest<ProductoDto>;
