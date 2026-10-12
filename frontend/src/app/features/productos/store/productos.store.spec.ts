import { TestBed } from '@angular/core/testing';
import { of, throwError } from 'rxjs';
import { Producto } from '../models/producto.model';
import { ProductosService } from '../services/productos.service';
import { ProductosStore } from './productos.store';

describe('ProductosStore', () => {
  let store: ProductosStore;
  let productosServiceMock: {
    getProductos: ReturnType<typeof vi.fn>;
    getProductosStockBajo: ReturnType<typeof vi.fn>;
    deleteProducto: ReturnType<typeof vi.fn>;
  };

  const mockProductos: Producto[] = [
    {
      id: 1,
      nombre: 'Laptop Gamer',
      descripcion: 'Alta gama',
      precio: 3500000,
      stock: 4,
      stockMinimo: 5,
      categoriaId: 1,
      categoriaNombre: 'Electrónica',
      isLowStock: true,
      fechaCreacion: '2026-03-01T10:00:00Z'
    },
    {
      id: 2,
      nombre: 'Mouse Inalámbrico',
      descripcion: 'Ergonómico',
      precio: 85000,
      stock: 20,
      stockMinimo: 10,
      categoriaId: 1,
      categoriaNombre: 'Electrónica',
      isLowStock: false,
      fechaCreacion: '2026-03-01T10:00:00Z'
    },
    {
      id: 3,
      nombre: 'Resma de Papel Carta',
      descripcion: '500 hojas',
      precio: 22000,
      stock: 3,
      stockMinimo: 10,
      categoriaId: 2,
      categoriaNombre: 'Oficina',
      isLowStock: true,
      fechaCreacion: '2026-03-01T10:00:00Z'
    }
  ];

  beforeEach(() => {
    productosServiceMock = {
      getProductos: vi.fn().mockReturnValue(of(mockProductos)),
      getProductosStockBajo: vi.fn().mockReturnValue(of([mockProductos[0], mockProductos[2]])),
      deleteProducto: vi.fn().mockReturnValue(of(undefined))
    };

    TestBed.configureTestingModule({
      providers: [
        ProductosStore,
        { provide: ProductosService, useValue: productosServiceMock }
      ]
    });

    store = TestBed.inject(ProductosStore);
  });

  it('debe inicializarse con estado por defecto', () => {
    expect(store.productos()).toEqual([]);
    expect(store.isLoading()).toBe(false);
    expect(store.error()).toBeNull();
    expect(store.soloStockBajo()).toBe(false);
    expect(store.selectedCategoriaId()).toBeNull();
    expect(store.searchTerm()).toBe('');
    expect(store.totalProductos()).toBe(0);
    expect(store.totalStockBajo()).toBe(0);
  });

  it('debe cargar productos exitosamente y computar totales', () => {
    store.loadProductos();

    expect(productosServiceMock.getProductos).toHaveBeenCalledTimes(1);
    expect(store.productos()).toEqual(mockProductos);
    expect(store.isLoading()).toBe(false);
    expect(store.error()).toBeNull();
    expect(store.totalProductos()).toBe(3);
    expect(store.totalStockBajo()).toBe(2);
  });

  it('debe registrar error cuando el servicio falla al cargar productos', () => {
    productosServiceMock.getProductos.mockReturnValue(throwError(() => new Error('Error de red')));

    store.loadProductos();

    expect(store.isLoading()).toBe(false);
    expect(store.error()).toBe('Error de red');
    expect(store.productos()).toEqual([]);
  });

  it('debe filtrar productos por stock bajo reactivamente', () => {
    store.loadProductos();

    store.setSoloStockBajo(true);
    expect(store.soloStockBajo()).toBe(true);
    expect(store.filteredProductos().length).toBe(2);
    expect(store.filteredProductos().every((p: Producto) => p.isLowStock || p.stock < p.stockMinimo)).toBe(true);

    store.setSoloStockBajo(false);
    expect(store.filteredProductos().length).toBe(3);
  });

  it('debe filtrar productos por categoría seleccionada', () => {
    store.loadProductos();

    store.setCategoriaFilter(2);
    expect(store.filteredProductos().length).toBe(1);
    expect(store.filteredProductos()[0].nombre).toBe('Resma de Papel Carta');

    store.setCategoriaFilter(null);
    expect(store.filteredProductos().length).toBe(3);
  });

  it('debe filtrar productos por término de búsqueda en nombre o descripción', () => {
    store.loadProductos();

    store.setSearchTerm('Gamer');
    expect(store.filteredProductos().length).toBe(1);
    expect(store.filteredProductos()[0].id).toBe(1);

    store.setSearchTerm('ergonómico');
    expect(store.filteredProductos().length).toBe(1);
    expect(store.filteredProductos()[0].id).toBe(2);

    store.setSearchTerm('');
    expect(store.filteredProductos().length).toBe(3);
  });

  it('debe remover el producto de la lista cuando se elimina con éxito', () => {
    store.loadProductos();
    expect(store.productos().length).toBe(3);

    store.deleteProducto(1);

    expect(productosServiceMock.deleteProducto).toHaveBeenCalledWith(1);
    expect(store.productos().length).toBe(2);
    expect(store.productos().some((p: Producto) => p.id === 1)).toBe(false);
  });
});
