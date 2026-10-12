import { ComponentFixture, TestBed } from '@angular/core/testing';
import { MAT_DIALOG_DATA, MatDialogRef } from '@angular/material/dialog';
import { of, throwError } from 'rxjs';
import { Producto } from '../../models/producto.model';
import { ProductosService } from '../../services/productos.service';
import { AjusteStockDialogComponent } from './ajuste-stock-dialog.component';

describe('AjusteStockDialogComponent', () => {
  let fixture: ComponentFixture<AjusteStockDialogComponent>;
  let component: AjusteStockDialogComponent;

  const mockProducto: Producto = {
    id: 5,
    nombre: 'Zapatos Deportivos',
    descripcion: 'Talla 42',
    precio: 210000,
    stock: 10,
    stockMinimo: 5,
    categoriaId: 1,
    categoriaNombre: 'Calzado',
    isLowStock: false,
    fechaCreacion: '2026-03-01T10:00:00Z'
  };

  let dialogRefMock: { close: ReturnType<typeof vi.fn> };
  let productosServiceMock: { adjustStock: ReturnType<typeof vi.fn> };

  beforeEach(async () => {
    dialogRefMock = { close: vi.fn() };
    productosServiceMock = {
      adjustStock: vi.fn().mockReturnValue(of({ ...mockProducto, stock: 15 }))
    };

    await TestBed.configureTestingModule({
      imports: [AjusteStockDialogComponent],
      providers: [
        { provide: MatDialogRef, useValue: dialogRefMock },
        { provide: MAT_DIALOG_DATA, useValue: { producto: mockProducto } },
        { provide: ProductosService, useValue: productosServiceMock }
      ]
    }).compileComponents();

    fixture = TestBed.createComponent(AjusteStockDialogComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('debe crearse e inicializar el formulario con el producto asignado', () => {
    expect(component).toBeTruthy();
    expect(component.producto.id).toBe(5);
    expect(component.form.controls.tipo.value).toBe(1); // 1 = Entrada
  });

  it('debe rechazar cantidades menores o iguales a 0', () => {
    component.form.controls.cantidad.setValue(0);
    expect(component.form.valid).toBe(false);
    expect(component.form.controls.cantidad.hasError('min')).toBe(true);
  });

  it('debe rechazar salidas que excedan el stock disponible (RF-7)', () => {
    component.form.controls.tipo.setValue(2); // 2 = Salida
    component.form.controls.cantidad.setValue(15); // Stock es 10
    expect(component.form.valid).toBe(false);
    expect(component.form.hasError('insufficientStock')).toBe(true);
  });

  it('debe enviar ajuste de ENTRADA exitosamente y cerrar el diálogo', () => {
    component.form.controls.tipo.setValue(1);
    component.form.controls.cantidad.setValue(5);

    component.onSubmit();

    expect(productosServiceMock.adjustStock).toHaveBeenCalledWith(5, { tipo: 1, cantidad: 5 });
    expect(dialogRefMock.close).toHaveBeenCalledWith(true);
  });

  it('debe enviar ajuste de SALIDA válida exitosamente y cerrar el diálogo', () => {
    productosServiceMock.adjustStock.mockReturnValue(of({ ...mockProducto, stock: 6 }));
    component.form.controls.tipo.setValue(2);
    component.form.controls.cantidad.setValue(4);

    component.onSubmit();

    expect(productosServiceMock.adjustStock).toHaveBeenCalledWith(5, { tipo: 2, cantidad: 4 });
    expect(dialogRefMock.close).toHaveBeenCalledWith(true);
  });

  it('debe capturar error cuando el servicio responde con fallo', () => {
    productosServiceMock.adjustStock.mockReturnValue(throwError(() => new Error('Stock insuficiente en servidor')));
    component.form.controls.tipo.setValue(1);
    component.form.controls.cantidad.setValue(5);

    component.onSubmit();

    expect(component.errorMessage()).toBe('Stock insuficiente en servidor');
    expect(dialogRefMock.close).not.toHaveBeenCalled();
  });
});
