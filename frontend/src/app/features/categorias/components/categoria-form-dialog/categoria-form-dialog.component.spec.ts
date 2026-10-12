import { TestBed } from '@angular/core/testing';
import { of, throwError } from 'rxjs';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { NoopAnimationsModule } from '@angular/platform-browser/animations';
import { CategoriaFormDialogComponent, CategoriaFormDialogData } from './categoria-form-dialog.component';
import { CategoriasService } from '../../services/categorias.service';
import { Categoria } from '../../models/categoria.model';

describe('CategoriaFormDialogComponent', () => {
  let dialogRefMock: { close: ReturnType<typeof vi.fn> };
  let serviceMock: {
    createCategoria: ReturnType<typeof vi.fn>;
    updateCategoria: ReturnType<typeof vi.fn>;
  };

  const createComponent = (data: CategoriaFormDialogData = {}) => {
    dialogRefMock = { close: vi.fn() };
    serviceMock = {
      createCategoria: vi.fn(),
      updateCategoria: vi.fn()
    };

    TestBed.configureTestingModule({
      imports: [CategoriaFormDialogComponent, NoopAnimationsModule],
      providers: [
        { provide: MatDialogRef, useValue: dialogRefMock },
        { provide: MAT_DIALOG_DATA, useValue: data },
        { provide: CategoriasService, useValue: serviceMock }
      ]
    });

    const fixture = TestBed.createComponent(CategoriaFormDialogComponent);
    const component = fixture.componentInstance;
    fixture.detectChanges();
    return { fixture, component };
  };

  afterEach(() => {
    TestBed.resetTestingModule();
  });

  it('debe inicializarse en modo creación cuando no se proporciona categoría', () => {
    const { component } = createComponent();

    expect(component.isEditMode).toBe(false);
    expect(component.form.controls.nombre.value).toBe('');
    expect(component.form.controls.activo.value).toBe(true);
    expect(component.form.valid).toBe(false);
  });

  it('debe inicializarse en modo edición cuando se pasa categoría existente', () => {
    const existing: Categoria = { id: 10, nombre: 'Tecnología', activo: false };
    const { component } = createComponent({ categoria: existing });

    expect(component.isEditMode).toBe(true);
    expect(component.form.controls.nombre.value).toBe('Tecnología');
    expect(component.form.controls.activo.value).toBe(false);
    expect(component.form.valid).toBe(true);
  });

  it('debe llamar a createCategoria y cerrar el modal al guardar en modo creación', () => {
    const created: Categoria = { id: 11, nombre: 'Jardín', activo: true };
    const { component } = createComponent();
    serviceMock.createCategoria.mockReturnValue(of(created));

    component.form.controls.nombre.setValue('Jardín');
    component.form.controls.activo.setValue(true);
    component.onSubmit();

    expect(serviceMock.createCategoria).toHaveBeenCalledWith({
      nombre: 'Jardín',
      activo: true
    });
    expect(dialogRefMock.close).toHaveBeenCalledWith(created);
  });

  it('debe llamar a updateCategoria y cerrar el modal al guardar en modo edición', () => {
    const existing: Categoria = { id: 5, nombre: 'Calzado', activo: true };
    const updated: Categoria = { id: 5, nombre: 'Calzado Deportivo', activo: true };
    const { component } = createComponent({ categoria: existing });
    serviceMock.updateCategoria.mockReturnValue(of(updated));

    component.form.controls.nombre.setValue('Calzado Deportivo');
    component.onSubmit();

    expect(serviceMock.updateCategoria).toHaveBeenCalledWith(5, {
      nombre: 'Calzado Deportivo',
      activo: true
    });
    expect(dialogRefMock.close).toHaveBeenCalledWith(updated);
  });

  it('debe manejar errores de servicio y mostrarlos en errorMessage', () => {
    const { component } = createComponent();
    serviceMock.createCategoria.mockReturnValue(throwError(() => ({
      error: { detail: 'Ya existe una categoría con este nombre' }
    })));

    component.form.controls.nombre.setValue('Duplicado');
    component.onSubmit();

    expect(component.isLoading()).toBe(false);
    expect(component.errorMessage()).toContain('Ya existe');
    expect(dialogRefMock.close).not.toHaveBeenCalled();
  });
});
