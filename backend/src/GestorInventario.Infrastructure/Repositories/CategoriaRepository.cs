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

    public async Task<bool> ExistsByNameAsync(string nombre, int? excludeId = null, CancellationToken cancellationToken = default)
    {
        var normalizedName = nombre.Trim().ToLower();
        var query = _context.Categorias.Where(c => c.Nombre.ToLower() == normalizedName);

        if (excludeId.HasValue)
        {
            query = query.Where(c => c.Id != excludeId.Value);
        }

        return await query.AnyAsync(cancellationToken);
    }

    public async Task<bool> HasProductsAsync(int id, CancellationToken cancellationToken = default)
    {
        return await _context.Productos.AnyAsync(p => p.CategoriaId == id, cancellationToken);
    }

    public async Task<Categoria?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return await _context.Categorias
            .FirstOrDefaultAsync(c => c.Id == id, cancellationToken);
    }

    public async Task<IReadOnlyList<Categoria>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Categorias
            .OrderBy(c => c.Nombre)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<Categoria>> GetAllActiveAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Categorias
            .Where(c => c.Activo)
            .OrderBy(c => c.Nombre)
            .ToListAsync(cancellationToken);
    }

    public async Task AddAsync(Categoria categoria, CancellationToken cancellationToken = default)
    {
        await _context.Categorias.AddAsync(categoria, cancellationToken);
    }

    public void Update(Categoria categoria)
    {
        _context.Categorias.Update(categoria);
    }

    public void Delete(Categoria categoria)
    {
        _context.Categorias.Remove(categoria);
    }
}
