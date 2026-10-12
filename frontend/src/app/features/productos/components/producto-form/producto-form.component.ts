import { ChangeDetectionStrategy, Component, inject, OnInit, signal } from '@angular/core';
import { FormControl, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { ActivatedRoute, Router, RouterLink } from '@angular/router';
import { MatButtonModule } from '@angular/material/button';
import { MatCardModule } from '@angular/material/card';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatIconModule } from '@angular/material/icon';
import { MatInputModule } from '@angular/material/input';
import { MatSelectModule } from '@angular/material/select';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { Categoria } from '../../models/categoria.model';
import { CrearProductoRequest, EditarProductoRequest, Producto } from '../../models/producto.model';
import { ProductosService } from '../../services/productos.service';

interface ProductoFormControls {
  nombre: FormControl<string>;
  descripcion: FormControl<string | null>;
  precio: FormControl<number | null>;
  stock: FormControl<number | null>;
  stockMinimo: FormControl<number | null>;
  categoriaId: FormControl<number | null>;
}

@Component({
  selector: 'app-producto-form',
  standalone: true,
  imports: [
    ReactiveFormsModule,
    MatFormFieldModule,
    MatInputModule,
    MatSelectModule,
    MatButtonModule,
    MatIconModule,
    MatCardModule,
    MatProgressSpinnerModule,
    RouterLink
  ],
  templateUrl: './producto-form.component.html',
  styleUrl: './producto-form.component.scss',
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class ProductoFormComponent implements OnInit {
  private readonly productosService = inject(ProductosService);
  private readonly router = inject(Router);
  private readonly route = inject(ActivatedRoute);

  readonly categorias = signal<Categoria[]>([]);
  readonly isEditMode = signal<boolean>(false);
  readonly isLoading = signal<boolean>(false);
  readonly errorMessage = signal<string | null>(null);
  readonly productoId = signal<number | null>(null);

  readonly form = new FormGroup<ProductoFormControls>({
    nombre: new FormControl('', {
      nonNullable: true,
      validators: [Validators.required, Validators.maxLength(150)]
    }),
    descripcion: new FormControl<string | null>(null, {
      validators: [Validators.maxLength(500)]
    }),
    precio: new FormControl<number | null>(null, {
      validators: [Validators.required, Validators.min(0.01)]
    }),
    stock: new FormControl<number | null>(null, {
      validators: [Validators.required, Validators.min(0)]
    }),
    stockMinimo: new FormControl<number | null>(null, {
      validators: [Validators.required, Validators.min(0)]
    }),
    categoriaId: new FormControl<number | null>(null, {
      validators: [Validators.required, Validators.min(1)]
    })
  });

  ngOnInit(): void {
    this.cargarCategorias();

    const idParam = this.route.snapshot.paramMap.get('id');
    if (idParam) {
      const id = Number(idParam);
      if (!Number.isNaN(id) && id > 0) {
        this.productoId.set(id);
        this.isEditMode.set(true);
        this.form.controls.stock.disable();
        this.cargarProductoParaEdicion(id);
      }
    }
  }

  private cargarCategorias(): void {
    this.productosService.getCategorias().subscribe({
      next: (data: Categoria[]) => this.categorias.set(data),
      error: () => this.errorMessage.set('No fue posible cargar las categorías.')
    });
  }

  private cargarProductoParaEdicion(id: number): void {
    this.isLoading.set(true);
    this.productosService.getProductoById(id).subscribe({
      next: (producto: Producto) => {
        this.form.patchValue({
          nombre: producto.nombre,
          descripcion: producto.descripcion,
          precio: producto.precio,
          stock: producto.stock,
          stockMinimo: producto.stockMinimo,
          categoriaId: producto.categoriaId
        });
        this.isLoading.set(false);
      },
      error: () => {
        this.errorMessage.set('No se encontró el producto a editar.');
        this.isLoading.set(false);
      }
    });
  }

  onSubmit(): void {
    if (this.form.invalid) {
      this.form.markAllAsTouched();
      return;
    }

    const formRaw = this.form.getRawValue();
    this.isLoading.set(true);
    this.errorMessage.set(null);

    if (this.isEditMode()) {
      const id = this.productoId();
      if (!id) return;

      const payload: EditarProductoRequest = {
        nombre: formRaw.nombre.trim(),
        descripcion: formRaw.descripcion ? formRaw.descripcion.trim() : null,
        precio: Number(formRaw.precio),
        stockMinimo: Number(formRaw.stockMinimo),
        categoriaId: Number(formRaw.categoriaId)
      };

      this.productosService.updateProducto(id, payload).subscribe({
        next: () => {
          this.isLoading.set(false);
          this.router.navigate(['/productos']);
        },
        error: (err: unknown) => {
          this.isLoading.set(false);
          this.errorMessage.set(err instanceof Error ? err.message : 'Error al actualizar producto');
        }
      });
    } else {
      const payload: CrearProductoRequest = {
        nombre: formRaw.nombre.trim(),
        descripcion: formRaw.descripcion ? formRaw.descripcion.trim() : null,
        precio: Number(formRaw.precio),
        stock: Number(formRaw.stock),
        stockMinimo: Number(formRaw.stockMinimo),
        categoriaId: Number(formRaw.categoriaId)
      };

      this.productosService.createProducto(payload).subscribe({
        next: () => {
          this.isLoading.set(false);
          this.router.navigate(['/productos']);
        },
        error: (err: unknown) => {
          this.isLoading.set(false);
          this.errorMessage.set(err instanceof Error ? err.message : 'Error al registrar producto');
        }
      });
    }
  }
}
