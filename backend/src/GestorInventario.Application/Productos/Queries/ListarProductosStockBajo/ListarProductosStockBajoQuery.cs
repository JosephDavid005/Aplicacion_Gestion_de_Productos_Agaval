using GestorInventario.Application.Productos.Common;
using MediatR;

namespace GestorInventario.Application.Productos.Queries.ListarProductosStockBajo;

public record ListarProductosStockBajoQuery : IRequest<IReadOnlyList<ProductoDto>>;
