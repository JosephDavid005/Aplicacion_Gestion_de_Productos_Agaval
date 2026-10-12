using MediatR;

namespace GestorInventario.Application.Categorias.Commands.EliminarCategoria;

public record EliminarCategoriaCommand(int Id) : IRequest<Unit>;
