using GestorInventario.Application.Productos.Common;
using MediatR;

namespace GestorInventario.Application.Productos.Queries.ListarProductos;

public record ListarProductosQuery : IRequest<IReadOnlyList<ProductoDto>>;
