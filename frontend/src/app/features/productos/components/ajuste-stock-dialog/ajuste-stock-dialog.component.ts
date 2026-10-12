import { ChangeDetectionStrategy, Component, inject, signal } from '@angular/core';
import { AbstractControl, FormControl, FormGroup, ReactiveFormsModule, ValidationErrors, Validators } from '@angular/forms';
import { MAT_DIALOG_DATA, MatDialogModule, MatDialogRef } from '@angular/material/dialog';
import { MatButtonModule } from '@angular/material/button';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatIconModule } from '@angular/material/icon';
import { MatInputModule } from '@angular/material/input';
import { MatRadioModule } from '@angular/material/radio';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { AjustarStockRequest, Producto, TipoMovimiento } from '../../models/producto.model';
import { ProductosService } from '../../services/productos.service';

export interface AjusteStockDialogData {
  producto: Producto;
}

interface AjusteStockFormControls {
  tipo: FormControl<TipoMovimiento>;
  cantidad: FormControl<number | null>;
}

@Component({
  selector: 'app-ajuste-stock-dialog',
  standalone: true,
  imports: [
    ReactiveFormsModule,
    MatDialogModule,
    MatButtonModule,
    MatFormFieldModule,
    MatInputModule,
    MatRadioModule,
    MatIconModule,
    MatProgressSpinnerModule
  ],
  templateUrl: './ajuste-stock-dialog.component.html',
  styleUrl: './ajuste-stock-dialog.component.scss',
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class AjusteStockDialogComponent {
  readonly dialogRef = inject(MatDialogRef<AjusteStockDialogComponent>);
  readonly data: AjusteStockDialogData = inject(MAT_DIALOG_DATA);
  private readonly productosService = inject(ProductosService);

  readonly producto: Producto = this.data.producto;
  readonly isLoading = signal<boolean>(false);
  readonly errorMessage = signal<string | null>(null);

  readonly form = new FormGroup<AjusteStockFormControls>(
    {
      tipo: new FormControl<TipoMovimiento>(1, {
        nonNullable: true,
        validators: [Validators.required]
      }),
      cantidad: new FormControl<number | null>(null, {
        validators: [Validators.required, Validators.min(1)]
      })
    },
    {
      validators: [
        (control: AbstractControl): ValidationErrors | null => {
          const tipo = control.get('tipo')?.value;
          const cantidad = control.get('cantidad')?.value;
          if (tipo === 2 && typeof cantidad === 'number' && cantidad > this.producto.stock) {
            return { insufficientStock: true };
          }
          return null;
        }
      ]
    }
  );

  onSubmit(): void {
    if (this.form.invalid) {
      this.form.markAllAsTouched();
      return;
    }

    const { tipo, cantidad } = this.form.getRawValue();
    if (!cantidad || cantidad <= 0) return;

    this.isLoading.set(true);
    this.errorMessage.set(null);

    const payload: AjustarStockRequest = {
      tipo,
      cantidad: Number(cantidad)
    };

    this.productosService.adjustStock(this.producto.id, payload).subscribe({
      next: () => {
        this.isLoading.set(false);
        this.dialogRef.close(true);
      },
      error: (err: unknown) => {
        this.isLoading.set(false);
        this.errorMessage.set(err instanceof Error ? err.message : 'Error al ajustar inventario');
      }
    });
  }

  onCancel(): void {
    this.dialogRef.close(false);
  }
}
