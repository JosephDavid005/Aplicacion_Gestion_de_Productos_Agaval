import { computed, inject, Injectable, signal } from '@angular/core';
import { Categoria, CreateCategoriaDto, UpdateCategoriaDto } from '../models/categoria.model';
import { CategoriasService } from '../services/categorias.service';

@Injectable({
  providedIn: 'root'
})
export class CategoriasStore {
  private readonly categoriasService = inject(CategoriasService);

  private readonly _categorias = signal<Categoria[]>([]);
  private readonly _isLoading = signal<boolean>(false);
  private readonly _error = signal<string | null>(null);
  private readonly _searchTerm = signal<string>('');

  readonly categorias = this._categorias.asReadonly();
  readonly isLoading = this._isLoading.asReadonly();
  readonly error = this._error.asReadonly();
  readonly searchTerm = this._searchTerm.asReadonly();

  readonly filteredCategorias = computed<Categoria[]>(() => {
    let result = this._categorias();
    const search = this._searchTerm().trim().toLowerCase();

    if (search.length > 0) {
      result = result.filter(c => c.nombre.toLowerCase().includes(search));
    }

    return result;
  });

  readonly totalCategorias = computed<number>(() => this._categorias().length);

  loadCategorias(): void {
    this._isLoading.set(true);
    this._error.set(null);

    this.categoriasService.getCategorias().subscribe({
      next: (data: Categoria[]) => {
        this._categorias.set(data);
        this._isLoading.set(false);
      },
      error: (err: unknown) => {
        const errorMsg = err instanceof Error ? err.message : 'Error al cargar categorías';
        this._error.set(errorMsg);
        this._isLoading.set(false);
      }
    });
  }

  setSearchTerm(term: string): void {
    this._searchTerm.set(term);
  }

  deleteCategoria(id: number): void {
    this._isLoading.set(true);
    this.categoriasService.deleteCategoria(id).subscribe({
      next: () => {
        this._categorias.update(items => items.filter(item => item.id !== id));
        this._isLoading.set(false);
      },
      error: (err: unknown) => {
        const errorMsg = err instanceof Error ? err.message : 'Error al eliminar categoría';
        this._error.set(errorMsg);
        this._isLoading.set(false);
      }
    });
  }
}
