import { ComponentFixture, TestBed } from '@angular/core/testing';
import { ActivatedRoute, provideRouter, Router } from '@angular/router';
import { of } from 'rxjs';
import { Categoria } from '../../models/categoria.model';
import { Producto } from '../../models/producto.model';
import { ProductosService } from '../../services/productos.service';
import { ProductoFormComponent } from './producto-form.component';

describe('ProductoFormComponent', () => {
  let fixture: ComponentFixture<ProductoFormComponent>;
  let component: ProductoFormComponent;
  let router: Router;

  const mockCategorias: Categoria[] = [
    { id: 1, nombre: 'Electrónica', activo: true },
    { id: 2, nombre: 'Oficina', activo: true }
  ];

  const mockProducto: Producto = {
    id: 10,
    nombre: 'Monitor 27 Pulgadas',
    descripcion: 'Panel IPS 144Hz',
    precio: 1200000,
    stock: 8,
    stockMinimo: 3,
    categoriaId: 1,
    categoriaNombre: 'Electrónica',
    isLowStock: false,
    fechaCreacion: '2026-03-01T10:00:00Z'
  };

  let productosServiceMock: {
    getCategorias: ReturnType<typeof vi.fn>;
    getProductoById: ReturnType<typeof vi.fn>;
    createProducto: ReturnType<typeof vi.fn>;
    updateProducto: ReturnType<typeof vi.fn>;
  };

  const setupTestBed = async (routeParams: Record<string, string> = {}) => {
    productosServiceMock = {
      getCategorias: vi.fn().mockReturnValue(of(mockCategorias)),
      getProductoById: vi.fn().mockReturnValue(of(mockProducto)),
      createProducto: vi.fn().mockReturnValue(of(mockProducto)),
      updateProducto: vi.fn().mockReturnValue(of(mockProducto))
    };

    await TestBed.configureTestingModule({
      imports: [ProductoFormComponent],
      providers: [
        provideRouter([]),
        { provide: ProductosService, useValue: productosServiceMock },
        {
          provide: ActivatedRoute,
          useValue: {
            snapshot: {
              paramMap: {
                get: (key: string) => routeParams[key] ?? null
              }
            }
          }
        }
      ]
    }).compileComponents();

    router = TestBed.inject(Router);
    vi.spyOn(router, 'navigate').mockResolvedValue(true);

    fixture = TestBed.createComponent(ProductoFormComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  };

  describe('Modo Creación (/productos/nuevo)', () => {
    beforeEach(async () => {
      await setupTestBed();
    });

    it('debe crearse, cargar categorías y definir modo edición en false', () => {
      expect(component).toBeTruthy();
      expect(component.isEditMode()).toBe(false);
      expect(productosServiceMock.getCategorias).toHaveBeenCalled();
      expect(component.categorias()).toEqual(mockCategorias);
    });

    it('debe marcar el formulario como inválido si los campos requeridos están vacíos', () => {
      expect(component.form.valid).toBe(false);

      const nombreControl = component.form.controls.nombre;
      expect(nombreControl.valid).toBe(false);
      expect(nombreControl.hasError('required')).toBe(true);

      const precioControl = component.form.controls.precio;
      expect(precioControl.valid).toBe(false);
      expect(precioControl.hasError('required')).toBe(true);
    });

    it('no debe enviar el formulario si es inválido (RF-11)', () => {
      component.onSubmit();
      expect(productosServiceMock.createProducto).not.toHaveBeenCalled();
    });

    it('debe enviar createProducto y navegar a /productos con datos válidos', () => {
      component.form.setValue({
        nombre: 'Nuevo Teclado',
        descripcion: 'Teclado inalámbrico',
        precio: 150000,
        stock: 10,
        stockMinimo: 5,
        categoriaId: 1
      });

      expect(component.form.valid).toBe(true);
      component.onSubmit();

      expect(productosServiceMock.createProducto).toHaveBeenCalledWith({
        nombre: 'Nuevo Teclado',
        descripcion: 'Teclado inalámbrico',
        precio: 150000,
        stock: 10,
        stockMinimo: 5,
        categoriaId: 1
      });
      expect(router.navigate).toHaveBeenCalledWith(['/productos']);
    });
  });

  describe('Modo Edición (/productos/:id/editar)', () => {
    beforeEach(async () => {
      await setupTestBed({ id: '10' });
    });

    it('debe identificar modo edición, cargar datos del producto y llenar el formulario', () => {
      expect(component.isEditMode()).toBe(true);
      expect(productosServiceMock.getProductoById).toHaveBeenCalledWith(10);
      expect(component.form.controls.nombre.value).toBe('Monitor 27 Pulgadas');
      expect(component.form.controls.precio.value).toBe(1200000);
      expect(component.form.controls.stock.disabled).toBe(true);
    });

    it('debe invocar updateProducto al guardar cambios en modo edición', () => {
      component.form.controls.nombre.setValue('Monitor 27 Pulgadas Editado');
      component.form.controls.precio.setValue(1350000);

      component.onSubmit();

      expect(productosServiceMock.updateProducto).toHaveBeenCalledWith(10, {
        nombre: 'Monitor 27 Pulgadas Editado',
        descripcion: 'Panel IPS 144Hz',
        precio: 1350000,
        stockMinimo: 3,
        categoriaId: 1
      });
      expect(router.navigate).toHaveBeenCalledWith(['/productos']);
    });
  });
});
