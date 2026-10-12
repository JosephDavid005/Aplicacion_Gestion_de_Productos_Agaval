import { ComponentFixture, TestBed } from '@angular/core/testing';
import { MAT_DIALOG_DATA, MatDialogRef } from '@angular/material/dialog';
import { of, throwError } from 'rxjs';
import { MovimientoInventario } from '../../models/movimiento-inventario.model';
import { Producto } from '../../models/producto.model';
import { ProductosService } from '../../services/productos.service';
import { MovimientosHistoricoDialogComponent } from './movimientos-historico-dialog.component';

describe('MovimientosHistoricoDialogComponent', () => {
  let fixture: ComponentFixture<MovimientosHistoricoDialogComponent>;
  let component: MovimientosHistoricoDialogComponent;

  const mockProducto: Producto = {
    id: 8,
    nombre: 'Zapatos Oxford',
    descripcion: 'Cuero genuino',
    precio: 320000,
    stock: 12,
    stockMinimo: 4,
    categoriaId: 1,
    categoriaNombre: 'Calzado',
    isLowStock: false,
    fechaCreacion: '2026-03-01T10:00:00Z'
  };

  const mockMovimientos: MovimientoInventario[] = [
    {
      id: 101,
      productoId: 8,
      tipoMovimiento: 'SALIDA',
      cantidad: 3,
      fecha: '2026-03-10T14:30:00Z',
      observacion: 'Venta presencial factura #501'
    },
    {
      id: 100,
      productoId: 8,
      tipoMovimiento: 'ENTRADA',
      cantidad: 15,
      fecha: '2026-03-01T10:00:00Z',
      observacion: 'Ingreso lote de producción'
    }
  ];

  let dialogRefMock: { close: ReturnType<typeof vi.fn> };
  let productosServiceMock: { getMovimientosByProductoId: ReturnType<typeof vi.fn> };

  beforeEach(async () => {
    dialogRefMock = { close: vi.fn() };
    productosServiceMock = {
      getMovimientosByProductoId: vi.fn().mockReturnValue(of(mockMovimientos))
    };

    await TestBed.configureTestingModule({
      imports: [MovimientosHistoricoDialogComponent],
      providers: [
        { provide: MatDialogRef, useValue: dialogRefMock },
        { provide: MAT_DIALOG_DATA, useValue: { producto: mockProducto } },
        { provide: ProductosService, useValue: productosServiceMock }
      ]
    }).compileComponents();

    fixture = TestBed.createComponent(MovimientosHistoricoDialogComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('debe crearse y consultar movimientos del producto al inicializar (RF-16)', () => {
    expect(component).toBeTruthy();
    expect(productosServiceMock.getMovimientosByProductoId).toHaveBeenCalledWith(8);
    expect(component.movimientos()).toEqual(mockMovimientos);
  });

  it('debe renderizar filas de movimientos con chips correspondientes para ENTRADA y SALIDA', () => {
    const compiled = fixture.nativeElement as HTMLElement;
    const rows = compiled.querySelectorAll('.c-movimiento-row');
    expect(rows.length).toBe(2);

    const entradaChip = compiled.querySelector('.c-mov-badge--entrada');
    expect(entradaChip).toBeTruthy();
    expect(entradaChip?.textContent).toContain('ENTRADA');

    const salidaChip = compiled.querySelector('.c-mov-badge--salida');
    expect(salidaChip).toBeTruthy();
    expect(salidaChip?.textContent).toContain('SALIDA');
  });

  it('debe mostrar estado vacío cuando no existen movimientos para el producto', () => {
    productosServiceMock.getMovimientosByProductoId.mockReturnValue(of([]));
    component.ngOnInit();
    fixture.detectChanges();

    const compiled = fixture.nativeElement as HTMLElement;
    const emptyEl = compiled.querySelector('.c-movimientos-empty');
    expect(emptyEl).toBeTruthy();
    expect(emptyEl?.textContent).toContain('No hay movimientos registrados');
  });

  it('debe mostrar alerta de error cuando el servicio falla', () => {
    productosServiceMock.getMovimientosByProductoId.mockReturnValue(throwError(() => new Error('Error al cargar historial')));
    component.ngOnInit();
    fixture.detectChanges();

    const compiled = fixture.nativeElement as HTMLElement;
    const errorEl = compiled.querySelector('.c-movimientos-error');
    expect(errorEl).toBeTruthy();
    expect(errorEl?.textContent).toContain('Error al cargar historial');
  });

  it('debe cerrar el diálogo al llamar onClose', () => {
    component.onClose();
    expect(dialogRefMock.close).toHaveBeenCalledTimes(1);
  });
});
