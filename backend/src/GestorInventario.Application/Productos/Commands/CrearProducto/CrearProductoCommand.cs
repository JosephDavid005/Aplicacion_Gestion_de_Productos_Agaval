using GestorInventario.Application.Productos.Common;
using MediatR;

namespace GestorInventario.Application.Productos.Commands.CrearProducto;

public record CrearProductoCommand(
    string Nombre,
    string? Descripcion,
    decimal Precio,
    int Stock,
    int StockMinimo,
    int CategoriaId
) : IRequest<ProductoDto>;
