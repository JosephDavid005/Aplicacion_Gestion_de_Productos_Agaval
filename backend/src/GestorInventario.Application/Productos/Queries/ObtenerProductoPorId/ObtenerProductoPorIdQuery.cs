using GestorInventario.Application.Productos.Common;
using MediatR;

namespace GestorInventario.Application.Productos.Queries.ObtenerProductoPorId;

public record ObtenerProductoPorIdQuery(int Id) : IRequest<ProductoDto>;
