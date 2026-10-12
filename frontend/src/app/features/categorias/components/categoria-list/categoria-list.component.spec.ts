import { TestBed } from '@angular/core/testing';
import { of } from 'rxjs';
import { signal } from '@angular/core';
import { MatDialog } from '@angular/material/dialog';
import { NoopAnimationsModule } from '@angular/platform-browser/animations';
import { CategoriaListComponent } from './categoria-list.component';
import { CategoriasStore } from '../../store/categorias.store';
import { Categoria } from '../../models/categoria.model';

describe('CategoriaListComponent', () => {
  let dialogMock: { open: ReturnType<typeof vi.fn> };
  let storeMock: {
    loadCategorias: ReturnType<typeof vi.fn>;
    setSearchTerm: ReturnType<typeof vi.fn>;
    deleteCategoria: ReturnType<typeof vi.fn>;
    filteredCategorias: ReturnType<typeof signal<Categoria[]>>;
    isLoading: ReturnType<typeof signal<boolean>>;
    error: ReturnType<typeof signal<string | null>>;
    searchTerm: ReturnType<typeof signal<string>>;
    totalCategorias: ReturnType<typeof signal<number>>;
  };

  const mockCategorias: Categoria[] = [
    { id: 1, nombre: 'Electrónica', activo: true },
    { id: 2, nombre: 'Hogar', activo: false }
  ];

  beforeEach(() => {
    dialogMock = {
      open: vi.fn().mockReturnValue({
        afterClosed: vi.fn().mockReturnValue(of(true))
      })
    };

    storeMock = {
      loadCategorias: vi.fn(),
      setSearchTerm: vi.fn(),
      deleteCategoria: vi.fn(),
      filteredCategorias: signal<Categoria[]>(mockCategorias),
      isLoading: signal<boolean>(false),
      error: signal<string | null>(null),
      searchTerm: signal<string>(''),
      totalCategorias: signal<number>(2)
    };

    TestBed.configureTestingModule({
      imports: [CategoriaListComponent, NoopAnimationsModule],
      providers: [
        { provide: MatDialog, useValue: dialogMock },
        { provide: CategoriasStore, useValue: storeMock }
      ]
    });
  });

  afterEach(() => {
    TestBed.resetTestingModule();
  });

  it('debe crearse y llamar a loadCategorias al inicializar', () => {
    const fixture = TestBed.createComponent(CategoriaListComponent);
    const component = fixture.componentInstance;
    fixture.detectChanges();

    expect(component).toBeTruthy();
    expect(storeMock.loadCategorias).toHaveBeenCalledTimes(1);
  });

  it('debe abrir el diálogo al solicitar crear categoría', () => {
    const fixture = TestBed.createComponent(CategoriaListComponent);
    const component = fixture.componentInstance;
    fixture.detectChanges();

    component.abrirCrearCategoria();

    expect(dialogMock.open).toHaveBeenCalled();
  });

  it('debe abrir el diálogo con la categoría seleccionada al editar', () => {
    const fixture = TestBed.createComponent(CategoriaListComponent);
    const component = fixture.componentInstance;
    fixture.detectChanges();

    component.abrirEditarCategoria(mockCategorias[0]);

    expect(dialogMock.open).toHaveBeenCalledWith(
      expect.anything(),
      expect.objectContaining({
        data: { categoria: mockCategorias[0] }
      })
    );
  });

  it('debe solicitar confirmación y llamar a deleteCategoria al eliminar', () => {
    const fixture = TestBed.createComponent(CategoriaListComponent);
    const component = fixture.componentInstance;
    fixture.detectChanges();

    vi.spyOn(window, 'confirm').mockReturnValue(true);

    component.eliminarCategoria(mockCategorias[0]);

    expect(storeMock.deleteCategoria).toHaveBeenCalledWith(1);
  });
});
