using GestorInventario.Application.Abstractions;
using GestorInventario.Domain.Entities;
using GestorInventario.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace GestorInventario.Infrastructure.Repositories;

public class CategoriaRepository : ICategoriaRepository
{
    private readonly PersistenceContext _context;

    public CategoriaRepository(PersistenceContext context)
    {
        _context = context;
    }

    public async Task<bool> ExistsAsync(int id, CancellationToken cancellationToken = default)
    {
        return await _context.Categorias.AnyAsync(c => c.Id == id && c.Activo, cancellationToken);
    }

    public async Task<Categoria?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return await _context.Categorias
            .FirstOrDefaultAsync(c => c.Id == id, cancellationToken);
    }

    public async Task<IReadOnlyList<Categoria>> GetAllActiveAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Categorias
            .Where(c => c.Activo)
            .OrderBy(c => c.Nombre)
            .ToListAsync(cancellationToken);
    }
}
