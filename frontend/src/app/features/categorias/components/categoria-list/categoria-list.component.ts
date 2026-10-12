import { ChangeDetectionStrategy, Component, inject, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { MatTableModule } from '@angular/material/table';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatInputModule } from '@angular/material/input';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatTooltipModule } from '@angular/material/tooltip';
import { MatDialog } from '@angular/material/dialog';
import { CategoriasStore } from '../../store/categorias.store';
import { Categoria } from '../../models/categoria.model';
import { CategoriaFormDialogComponent } from '../categoria-form-dialog/categoria-form-dialog.component';

@Component({
  selector: 'app-categoria-list',
  standalone: true,
  imports: [
    CommonModule,
    MatTableModule,
    MatButtonModule,
    MatIconModule,
    MatInputModule,
    MatFormFieldModule,
    MatProgressSpinnerModule,
    MatTooltipModule
  ],
  templateUrl: './categoria-list.component.html',
  styleUrl: './categoria-list.component.scss',
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class CategoriaListComponent implements OnInit {
  readonly store = inject(CategoriasStore);
  private readonly dialog = inject(MatDialog);

  readonly displayedColumns: string[] = ['id', 'nombre', 'estado', 'acciones'];

  ngOnInit(): void {
    this.store.loadCategorias();
  }

  onSearchChange(event: Event): void {
    const input = event.target as HTMLInputElement;
    this.store.setSearchTerm(input.value);
  }

  abrirCrearCategoria(): void {
    const dialogRef = this.dialog.open(CategoriaFormDialogComponent, {
      width: '450px',
      disableClose: true
    });

    dialogRef.afterClosed().subscribe(result => {
      if (result) {
        this.store.loadCategorias();
      }
    });
  }

  abrirEditarCategoria(categoria: Categoria): void {
    const dialogRef = this.dialog.open(CategoriaFormDialogComponent, {
      width: '450px',
      disableClose: true,
      data: { categoria }
    });

    dialogRef.afterClosed().subscribe(result => {
      if (result) {
        this.store.loadCategorias();
      }
    });
  }

  eliminarCategoria(categoria: Categoria): void {
    const confirmed = window.confirm(
      `¿Está seguro de que desea eliminar la categoría "${categoria.nombre}"?`
    );

    if (confirmed) {
      this.store.deleteCategoria(categoria.id);
    }
  }
}
