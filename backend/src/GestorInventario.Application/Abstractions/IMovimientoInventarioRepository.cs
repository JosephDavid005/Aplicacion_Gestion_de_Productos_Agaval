using GestorInventario.Domain.Entities;

namespace GestorInventario.Application.Abstractions;

public interface IMovimientoInventarioRepository
{
    Task AddAsync(MovimientoInventario movimiento, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<MovimientoInventario>> GetByProductoIdAsync(int productoId, CancellationToken cancellationToken = default);
}
