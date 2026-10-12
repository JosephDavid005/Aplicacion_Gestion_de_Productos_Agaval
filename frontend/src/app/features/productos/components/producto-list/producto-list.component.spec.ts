import { provideRouter } from '@angular/router';
import { ComponentFixture, TestBed } from '@angular/core/testing';
import { signal } from '@angular/core';
import { MatDialog } from '@angular/material/dialog';
import { of } from 'rxjs';
import { Categoria } from '../../models/categoria.model';
import { Producto } from '../../models/producto.model';
import { ProductosService } from '../../services/productos.service';
import { ProductosStore } from '../../store/productos.store';
import { ProductoListComponent } from './producto-list.component';

describe('ProductoListComponent', () => {
  let fixture: ComponentFixture<ProductoListComponent>;
  let component: ProductoListComponent;

  const mockProductos: Producto[] = [
    {
      id: 1,
      nombre: 'Laptop Gamer',
      descripcion: 'Portátil de alta gama',
      precio: 4500000,
      stock: 2,
      stockMinimo: 5,
      categoriaId: 1,
      categoriaNombre: 'Electrónica',
      isLowStock: true,
      fechaCreacion: '2026-03-01T10:00:00Z'
    },
    {
      id: 2,
      nombre: 'Teclado Mecánico',
      descripcion: 'RGB Switches Red',
      precio: 250000,
      stock: 15,
      stockMinimo: 5,
      categoriaId: 1,
      categoriaNombre: 'Electrónica',
      isLowStock: false,
      fechaCreacion: '2026-03-01T10:00:00Z'
    }
  ];

  const mockCategorias: Categoria[] = [
    { id: 1, nombre: 'Electrónica', activo: true },
    { id: 2, nombre: 'Oficina', activo: true }
  ];

  let storeMock: {
    filteredProductos: ReturnType<typeof signal<Producto[]>>;
    isLoading: ReturnType<typeof signal<boolean>>;
    error: ReturnType<typeof signal<string | null>>;
    totalProductos: ReturnType<typeof signal<number>>;
    totalStockBajo: ReturnType<typeof signal<number>>;
    selectedCategoriaId: ReturnType<typeof signal<number | null>>;
    soloStockBajo: ReturnType<typeof signal<boolean>>;
    searchTerm: ReturnType<typeof signal<string>>;
    loadProductos: ReturnType<typeof vi.fn>;
    deleteProducto: ReturnType<typeof vi.fn>;
    setCategoriaFilter: ReturnType<typeof vi.fn>;
    setSoloStockBajo: ReturnType<typeof vi.fn>;
    setSearchTerm: ReturnType<typeof vi.fn>;
  };

  let productosServiceMock: {
    getCategorias: ReturnType<typeof vi.fn>;
  };

  let dialogMock: {
    open: ReturnType<typeof vi.fn>;
  };

  beforeEach(async () => {
    storeMock = {
      filteredProductos: signal<Producto[]>(mockProductos),
      isLoading: signal<boolean>(false),
      error: signal<string | null>(null),
      totalProductos: signal<number>(2),
      totalStockBajo: signal<number>(1),
      selectedCategoriaId: signal<number | null>(null),
      soloStockBajo: signal<boolean>(false),
      searchTerm: signal<string>(''),
      loadProductos: vi.fn(),
      deleteProducto: vi.fn(),
      setCategoriaFilter: vi.fn(),
      setSoloStockBajo: vi.fn(),
      setSearchTerm: vi.fn()
    };

    productosServiceMock = {
      getCategorias: vi.fn().mockReturnValue(of(mockCategorias))
    };

    dialogMock = {
      open: vi.fn().mockReturnValue({
        afterClosed: () => of(true)
      })
    };

    await TestBed.configureTestingModule({
      imports: [ProductoListComponent],
      providers: [
        provideRouter([]),
        { provide: ProductosStore, useValue: storeMock },
        { provide: ProductosService, useValue: productosServiceMock },
        { provide: MatDialog, useValue: dialogMock }
      ]
    }).compileComponents();

    fixture = TestBed.createComponent(ProductoListComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('debe crearse y llamar a loadProductos al iniciar', () => {
    expect(component).toBeTruthy();
    expect(storeMock.loadProductos).toHaveBeenCalledTimes(1);
    expect(productosServiceMock.getCategorias).toHaveBeenCalledTimes(1);
    expect(component.categorias()).toEqual(mockCategorias);
  });

  it('debe renderizar filas de la tabla con los productos cargados', () => {
    const compiled = fixture.nativeElement as HTMLElement;
    const rows = compiled.querySelectorAll('.c-producto-table__row');
    expect(rows.length).toBe(2);
    expect(compiled.textContent).toContain('Laptop Gamer');
    expect(compiled.textContent).toContain('Teclado Mecánico');
  });

  it('debe mostrar indicador visual de stock bajo (c-stock-badge--low) para productos con stock < stockMinimo', () => {
    const compiled = fixture.nativeElement as HTMLElement;
    const lowStockBadges = compiled.querySelectorAll('.c-stock-badge--low');
    expect(lowStockBadges.length).toBe(1);
    expect(lowStockBadges[0].textContent).toContain('Stock Bajo');
  });

  it('debe mostrar indicador normal (c-stock-badge--normal) para productos con stock suficiente', () => {
    const compiled = fixture.nativeElement as HTMLElement;
    const normalBadges = compiled.querySelectorAll('.c-stock-badge--normal');
    expect(normalBadges.length).toBe(1);
    expect(normalBadges[0].textContent).toContain('Normal');
  });

  it('debe permitir alternar el filtro de solo stock bajo (RF-12)', () => {
    component.toggleStockBajo();
    expect(storeMock.setSoloStockBajo).toHaveBeenCalledWith(true);
  });

  it('debe permitir filtrar por categoría (RF-12)', () => {
    component.onCategoriaChange(2);
    expect(storeMock.setCategoriaFilter).toHaveBeenCalledWith(2);
  });

  it('debe abrir diálogo de ajuste de stock al invocar openAjusteStock (RF-7)', () => {
    component.openAjusteStock(mockProductos[0]);
    expect(dialogMock.open).toHaveBeenCalled();
    expect(storeMock.loadProductos).toHaveBeenCalledTimes(2); // Initial + reload on success
  });

  it('debe abrir diálogo de historial de movimientos al invocar openHistorialMovimientos (RF-16)', () => {
    component.openHistorialMovimientos(mockProductos[0]);
    expect(dialogMock.open).toHaveBeenCalled();
  });

  it('debe mostrar mensaje cuando no hay productos disponibles', () => {
    storeMock.filteredProductos.set([]);
    fixture.detectChanges();

    const compiled = fixture.nativeElement as HTMLElement;
    const emptyMessage = compiled.querySelector('.c-producto-list__empty');
    expect(emptyMessage).toBeTruthy();
    expect(emptyMessage?.textContent).toContain('No se encontraron productos');
  });

  it('debe mostrar spinner cuando isLoading es true', () => {
    storeMock.isLoading.set(true);
    fixture.detectChanges();

    const compiled = fixture.nativeElement as HTMLElement;
    const loadingEl = compiled.querySelector('.c-producto-list__loading');
    expect(loadingEl).toBeTruthy();
  });

  it('debe mostrar alerta de error cuando store.error contiene un mensaje', () => {
    storeMock.error.set('Fallo de conexión');
    fixture.detectChanges();

    const compiled = fixture.nativeElement as HTMLElement;
    const errorEl = compiled.querySelector('.c-producto-list__error');
    expect(errorEl).toBeTruthy();
    expect(errorEl?.textContent).toContain('Fallo de conexión');
  });

  it('debe inicializar dataSource con MatPaginator y MatSort vinculados (RF-21, RF-22)', () => {
    expect(component.dataSource).toBeDefined();
    expect(component.dataSource.data).toEqual(mockProductos);
    const compiled = fixture.nativeElement as HTMLElement;
    expect(compiled.querySelector('mat-paginator')).toBeTruthy();
  });
});

