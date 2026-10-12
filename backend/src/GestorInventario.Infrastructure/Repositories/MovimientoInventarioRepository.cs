using GestorInventario.Application.Abstractions;
using GestorInventario.Domain.Entities;
using GestorInventario.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace GestorInventario.Infrastructure.Repositories;

public class MovimientoInventarioRepository : IMovimientoInventarioRepository
{
    private readonly PersistenceContext _context;

    public MovimientoInventarioRepository(PersistenceContext context)
    {
        _context = context;
    }

    public async Task AddAsync(MovimientoInventario movimiento, CancellationToken cancellationToken = default)
    {
        await _context.MovimientosInventario.AddAsync(movimiento, cancellationToken);
    }

    public async Task<IReadOnlyList<MovimientoInventario>> GetByProductoIdAsync(int productoId, CancellationToken cancellationToken = default)
    {
        return await _context.MovimientosInventario
            .AsNoTracking()
            .Where(m => m.ProductoId == productoId)
            .OrderByDescending(m => m.Fecha)
            .ToListAsync(cancellationToken);
    }
}
