import { computed, inject, Injectable, signal } from '@angular/core';
import { Producto } from '../models/producto.model';
import { ProductosService } from '../services/productos.service';

@Injectable({
  providedIn: 'root'
})
export class ProductosStore {
  private readonly productosService = inject(ProductosService);

  private readonly _productos = signal<Producto[]>([]);
  private readonly _isLoading = signal<boolean>(false);
  private readonly _error = signal<string | null>(null);
  private readonly _soloStockBajo = signal<boolean>(false);
  private readonly _selectedCategoriaId = signal<number | null>(null);
  private readonly _searchTerm = signal<string>('');

  readonly productos = this._productos.asReadonly();
  readonly isLoading = this._isLoading.asReadonly();
  readonly error = this._error.asReadonly();
  readonly soloStockBajo = this._soloStockBajo.asReadonly();
  readonly selectedCategoriaId = this._selectedCategoriaId.asReadonly();
  readonly searchTerm = this._searchTerm.asReadonly();

  readonly filteredProductos = computed<Producto[]>(() => {
    let result = this._productos();

    if (this._soloStockBajo()) {
      result = result.filter(p => p.isLowStock || p.stock < p.stockMinimo);
    }

    const catId = this._selectedCategoriaId();
    if (catId !== null) {
      result = result.filter(p => p.categoriaId === catId);
    }

    const search = this._searchTerm().trim().toLowerCase();
    if (search.length > 0) {
      result = result.filter(p =>
        p.nombre.toLowerCase().includes(search) ||
        (p.descripcion !== null && p.descripcion.toLowerCase().includes(search))
      );
    }

    return result;
  });

  readonly totalProductos = computed<number>(() => this._productos().length);

  readonly totalStockBajo = computed<number>(() =>
    this._productos().filter(p => p.isLowStock || p.stock < p.stockMinimo).length
  );

  loadProductos(): void {
    this._isLoading.set(true);
    this._error.set(null);

    this.productosService.getProductos().subscribe({
      next: (data: Producto[]) => {
        this._productos.set(data);
        this._isLoading.set(false);
      },
      error: (err: unknown) => {
        const errorMsg = err instanceof Error ? err.message : 'Error al cargar productos';
        this._error.set(errorMsg);
        this._isLoading.set(false);
      }
    });
  }

  setSoloStockBajo(value: boolean): void {
    this._soloStockBajo.set(value);
  }

  setCategoriaFilter(categoriaId: number | null): void {
    this._selectedCategoriaId.set(categoriaId);
  }

  setSearchTerm(term: string): void {
    this._searchTerm.set(term);
  }

  deleteProducto(id: number): void {
    this._isLoading.set(true);
    this.productosService.deleteProducto(id).subscribe({
      next: () => {
        this._productos.update(items => items.filter(item => item.id !== id));
        this._isLoading.set(false);
      },
      error: (err: unknown) => {
        const errorMsg = err instanceof Error ? err.message : 'Error al eliminar producto';
        this._error.set(errorMsg);
        this._isLoading.set(false);
      }
    });
  }
}
