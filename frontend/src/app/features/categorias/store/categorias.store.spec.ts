import { TestBed } from '@angular/core/testing';
import { of, throwError } from 'rxjs';
import { CategoriasStore } from './categorias.store';
import { CategoriasService } from '../services/categorias.service';
import { Categoria } from '../models/categoria.model';

describe('CategoriasStore', () => {
  let store: CategoriasStore;
  let serviceMock: {
    getCategorias: ReturnType<typeof vi.fn>;
    createCategoria: ReturnType<typeof vi.fn>;
    updateCategoria: ReturnType<typeof vi.fn>;
    deleteCategoria: ReturnType<typeof vi.fn>;
  };

  const mockCategorias: Categoria[] = [
    { id: 1, nombre: 'Electrónica', activo: true },
    { id: 2, nombre: 'Oficina', activo: true },
    { id: 3, nombre: 'Hogar', activo: false }
  ];

  beforeEach(() => {
    serviceMock = {
      getCategorias: vi.fn().mockReturnValue(of(mockCategorias)),
      createCategoria: vi.fn().mockReturnValue(of(mockCategorias[0])),
      updateCategoria: vi.fn().mockReturnValue(of(mockCategorias[0])),
      deleteCategoria: vi.fn().mockReturnValue(of(undefined))
    };

    TestBed.configureTestingModule({
      providers: [
        CategoriasStore,
        { provide: CategoriasService, useValue: serviceMock }
      ]
    });

    store = TestBed.inject(CategoriasStore);
  });

  it('debe inicializarse con estados por defecto', () => {
    expect(store.categorias()).toEqual([]);
    expect(store.isLoading()).toBe(false);
    expect(store.error()).toBeNull();
    expect(store.searchTerm()).toBe('');
    expect(store.totalCategorias()).toBe(0);
  });

  it('loadCategorias debe actualizar signals correctamente en caso de éxito', () => {
    store.loadCategorias();

    expect(store.categorias()).toEqual(mockCategorias);
    expect(store.isLoading()).toBe(false);
    expect(store.error()).toBeNull();
    expect(store.totalCategorias()).toBe(3);
  });

  it('loadCategorias debe registrar mensaje de error si falla', () => {
    serviceMock.getCategorias.mockReturnValue(throwError(() => new Error('Error de red')));

    store.loadCategorias();

    expect(store.categorias()).toEqual([]);
    expect(store.isLoading()).toBe(false);
    expect(store.error()).toBe('Error de red');
  });

  it('filteredCategorias debe filtrar por término de búsqueda (case insensitive)', () => {
    store.loadCategorias();

    store.setSearchTerm('elec');
    expect(store.filteredCategorias().length).toBe(1);
    expect(store.filteredCategorias()[0].nombre).toBe('Electrónica');

    store.setSearchTerm('OFICINA');
    expect(store.filteredCategorias().length).toBe(1);
    expect(store.filteredCategorias()[0].nombre).toBe('Oficina');

    store.setSearchTerm('');
    expect(store.filteredCategorias().length).toBe(3);
  });

  it('deleteCategoria debe remover la categoría del signal cuando el servicio completa', () => {
    store.loadCategorias();
    expect(store.categorias().length).toBe(3);

    store.deleteCategoria(2);

    expect(store.categorias().length).toBe(2);
    expect(store.categorias().some((c: Categoria) => c.id === 2)).toBe(false);
    expect(store.isLoading()).toBe(false);
  });
});
