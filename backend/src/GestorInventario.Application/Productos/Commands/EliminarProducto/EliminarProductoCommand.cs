using MediatR;

namespace GestorInventario.Application.Productos.Commands.EliminarProducto;

public record EliminarProductoCommand(int Id) : IRequest<Unit>;
