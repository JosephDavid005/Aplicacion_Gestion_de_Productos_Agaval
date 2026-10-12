using GestorInventario.Application.Abstractions;
using GestorInventario.Domain.Entities;
using GestorInventario.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace GestorInventario.Infrastructure.Persistence;

public class PersistenceContext : DbContext, IUnitOfWork
{
    public PersistenceContext(DbContextOptions<PersistenceContext> options) : base(options)
    {
    }

    public DbSet<Categoria> Categorias => Set<Categoria>();
    public DbSet<Producto> Productos => Set<Producto>();
    public DbSet<MovimientoInventario> MovimientosInventario => Set<MovimientoInventario>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Categoria>(entity =>
        {
            entity.ToTable("Categorias");

            entity.HasKey(c => c.Id)
                .HasName("PK_Categorias");

            entity.Property(c => c.Id)
                .ValueGeneratedOnAdd();

            entity.Property(c => c.Nombre)
                .HasMaxLength(100)
                .IsRequired();

            entity.Property(c => c.Activo)
                .HasDefaultValue(true)
                .IsRequired();
        });

        modelBuilder.Entity<Producto>(entity =>
        {
            entity.ToTable("Productos", t =>
            {
                t.HasCheckConstraint("CK_Productos_Stock", "[Stock] >= 0");
            });

            entity.HasKey(p => p.Id)
                .HasName("PK_Productos");

            entity.Property(p => p.Id)
                .ValueGeneratedOnAdd();

            entity.Property(p => p.Nombre)
                .HasMaxLength(150)
                .IsRequired();

            entity.Property(p => p.Descripcion)
                .HasMaxLength(500)
                .IsRequired(false);

            entity.Property(p => p.Precio)
                .HasColumnType("decimal(10,2)")
                .IsRequired();

            entity.Property(p => p.Stock)
                .HasDefaultValue(0)
                .IsRequired();

            entity.Property(p => p.StockMinimo)
                .HasDefaultValue(5)
                .IsRequired();

            entity.Property(p => p.CategoriaId)
                .IsRequired();

            entity.Property(p => p.FechaCreacion)
                .HasColumnType("datetime2")
                .HasDefaultValueSql("GETDATE()")
                .IsRequired();

            entity.Ignore(p => p.IsLowStock);

            entity.HasOne(p => p.Categoria)
                .WithMany(c => c.Productos)
                .HasForeignKey(p => p.CategoriaId)
                .HasConstraintName("FK_Productos_Categorias")
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<MovimientoInventario>(entity =>
        {
            entity.ToTable("MovimientosInventario", t =>
            {
                t.HasCheckConstraint("CK_Movimientos_Tipo", "[TipoMovimiento] IN ('ENTRADA', 'SALIDA')");
            });

            entity.HasKey(m => m.Id)
                .HasName("PK_MovimientosInventario");

            entity.Property(m => m.Id)
                .ValueGeneratedOnAdd();

            entity.Property(m => m.ProductoId)
                .IsRequired();

            entity.Property(m => m.TipoMovimiento)
                .HasConversion(
                    v => v == TipoMovimiento.Entrada ? "ENTRADA" : "SALIDA",
                    v => v == "ENTRADA" ? TipoMovimiento.Entrada : TipoMovimiento.Salida)
                .HasMaxLength(10)
                .IsUnicode(false)
                .IsRequired();

            entity.Property(m => m.Cantidad)
                .IsRequired();

            entity.Property(m => m.Fecha)
                .HasColumnType("datetime2")
                .HasDefaultValueSql("GETDATE()")
                .IsRequired();

            entity.Property(m => m.Observacion)
                .HasMaxLength(255)
                .IsRequired(false);

            entity.HasOne(m => m.Producto)
                .WithMany()
                .HasForeignKey(m => m.ProductoId)
                .HasConstraintName("FK_Movimientos_Productos")
                .OnDelete(DeleteBehavior.Cascade);
        });
    }

    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return base.SaveChangesAsync(cancellationToken);
    }
}
