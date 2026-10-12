import { ChangeDetectionStrategy, Component, inject, signal } from '@angular/core';
import { FormControl, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { MAT_DIALOG_DATA, MatDialogModule, MatDialogRef } from '@angular/material/dialog';
import { MatButtonModule } from '@angular/material/button';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatIconModule } from '@angular/material/icon';
import { MatInputModule } from '@angular/material/input';
import { MatSlideToggleModule } from '@angular/material/slide-toggle';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { Categoria, CreateCategoriaDto, UpdateCategoriaDto } from '../../models/categoria.model';
import { CategoriasService } from '../../services/categorias.service';

export interface CategoriaFormDialogData {
  categoria?: Categoria;
}

interface CategoriaFormControls {
  nombre: FormControl<string>;
  activo: FormControl<boolean>;
}

@Component({
  selector: 'app-categoria-form-dialog',
  standalone: true,
  imports: [
    ReactiveFormsModule,
    MatDialogModule,
    MatButtonModule,
    MatFormFieldModule,
    MatInputModule,
    MatSlideToggleModule,
    MatIconModule,
    MatProgressSpinnerModule
  ],
  templateUrl: './categoria-form-dialog.component.html',
  styleUrl: './categoria-form-dialog.component.scss',
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class CategoriaFormDialogComponent {
  readonly dialogRef = inject(MatDialogRef<CategoriaFormDialogComponent>);
  readonly data: CategoriaFormDialogData = inject(MAT_DIALOG_DATA, { optional: true }) ?? {};
  private readonly categoriasService = inject(CategoriasService);

  readonly isEditMode = Boolean(this.data?.categoria);
  readonly isLoading = signal<boolean>(false);
  readonly errorMessage = signal<string | null>(null);

  readonly form = new FormGroup<CategoriaFormControls>({
    nombre: new FormControl<string>(this.data?.categoria?.nombre ?? '', {
      nonNullable: true,
      validators: [Validators.required, Validators.maxLength(100)]
    }),
    activo: new FormControl<boolean>(this.data?.categoria?.activo ?? true, {
      nonNullable: true
    })
  });

  onSubmit(): void {
    if (this.form.invalid) {
      this.form.markAllAsTouched();
      return;
    }

    this.isLoading.set(true);
    this.errorMessage.set(null);

    const nombre = this.form.controls.nombre.value.trim();
    const activo = this.form.controls.activo.value;

    if (this.isEditMode && this.data.categoria) {
      const updateDto: UpdateCategoriaDto = { nombre, activo };
      this.categoriasService.updateCategoria(this.data.categoria.id, updateDto).subscribe({
        next: (updated: Categoria) => {
          this.isLoading.set(false);
          this.dialogRef.close(updated);
        },
        error: (err: unknown) => {
          this.handleError(err, 'Error al actualizar la categoría');
        }
      });
    } else {
      const createDto: CreateCategoriaDto = { nombre, activo };
      this.categoriasService.createCategoria(createDto).subscribe({
        next: (created: Categoria) => {
          this.isLoading.set(false);
          this.dialogRef.close(created);
        },
        error: (err: unknown) => {
          this.handleError(err, 'Error al crear la categoría');
        }
      });
    }
  }

  private handleError(err: unknown, defaultMsg: string): void {
    let msg = defaultMsg;
    if (typeof err === 'object' && err !== null) {
      const e = err as { error?: { detail?: string; title?: string }; message?: string };
      msg = e.error?.detail ?? e.error?.title ?? e.message ?? defaultMsg;
    }
    this.errorMessage.set(msg);
    this.isLoading.set(false);
  }
}
