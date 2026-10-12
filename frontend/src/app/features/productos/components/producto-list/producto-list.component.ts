import { ChangeDetectionStrategy, Component, computed, effect, inject, OnInit, signal, ViewChild } from '@angular/core';
import { CurrencyPipe, DatePipe } from '@angular/common';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatTableDataSource, MatTableModule } from '@angular/material/table';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatTooltipModule } from '@angular/material/tooltip';
import { MatDialog } from '@angular/material/dialog';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatSelectModule } from '@angular/material/select';
import { MatPaginator, MatPaginatorModule } from '@angular/material/paginator';
import { MatSort, MatSortModule } from '@angular/material/sort';
import { RouterLink } from '@angular/router';
import { Categoria } from '../../models/categoria.model';
import { Producto } from '../../models/producto.model';
import { ProductosService } from '../../services/productos.service';
import { ProductosStore } from '../../store/productos.store';
import { AjusteStockDialogComponent } from '../ajuste-stock-dialog/ajuste-stock-dialog.component';
import { MovimientosHistoricoDialogComponent } from '../movimientos-historico-dialog/movimientos-historico-dialog.component';

@Component({
  selector: 'app-producto-list',
  standalone: true,
  imports: [
    CurrencyPipe,
    DatePipe,
    MatTableModule,
    MatButtonModule,
    MatIconModule,
    MatProgressSpinnerModule,
    MatTooltipModule,
    MatFormFieldModule,
    MatInputModule,
    MatSelectModule,
    MatPaginatorModule,
    MatSortModule,
    RouterLink
  ],
  templateUrl: './producto-list.component.html',
  styleUrl: './producto-list.component.scss',
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class ProductoListComponent implements OnInit {
  protected readonly store = inject(ProductosStore);
  private readonly productosService = inject(ProductosService);
  private readonly dialog = inject(MatDialog);

  readonly categorias = signal<Categoria[]>([]);
  readonly dataSource = new MatTableDataSource<Producto>([]);

  readonly pageSizeOptions = computed<number[]>(() => {
    const total = this.store.filteredProductos().length;
    if (total <= 10) {
      return [];
    }
    // Incrementos de 5 en 5 (5, 10, 15, 20...)
    const maxOption = Math.min(Math.ceil(total / 5) * 5, 50);
    const options: number[] = [];
    for (let step = 5; step <= Math.max(maxOption, 15); step += 5) {
      options.push(step);
    }
    return options;
  });

  @ViewChild(MatPaginator) set paginator(paginator: MatPaginator | undefined) {
    this.dataSource.paginator = paginator ?? null;
  }

  @ViewChild(MatSort) set sort(sort: MatSort | undefined) {
    if (sort) {
      this.dataSource.sort = sort;
    }
  }

  readonly displayedColumns: string[] = [
    'nombre',
    'categoria',
    'precio',
    'stock',
    'fechaCreacion',
    'acciones'
  ];

  constructor() {
    effect(() => {
      this.dataSource.data = this.store.filteredProductos();
    });
  }

  ngOnInit(): void {
    this.store.loadProductos();
    this.cargarCategorias();
  }

  private cargarCategorias(): void {
    this.productosService.getCategorias().subscribe({
      next: (data: Categoria[]) => this.categorias.set(data),
      error: () => {}
    });
  }

  isLowStock(producto: Producto): boolean {
    return producto.isLowStock || producto.stock < producto.stockMinimo;
  }

  openAjusteStock(producto: Producto): void {
    const dialogRef = this.dialog.open(AjusteStockDialogComponent, {
      width: '440px',
      data: { producto }
    });

    dialogRef.afterClosed().subscribe((result: boolean) => {
      if (result) {
        this.store.loadProductos();
      }
    });
  }

  openHistorialMovimientos(producto: Producto): void {
    this.dialog.open(MovimientosHistoricoDialogComponent, {
      width: '650px',
      data: { producto }
    });
  }

  onSearchChange(event: Event): void {
    const input = event.target as HTMLInputElement;
    this.store.setSearchTerm(input.value);
  }

  onCategoriaChange(categoriaId: number | null): void {
    this.store.setCategoriaFilter(categoriaId);
  }

  toggleStockBajo(): void {
    this.store.setSoloStockBajo(!this.store.soloStockBajo());
  }

  limpiarFiltros(): void {
    this.store.setSearchTerm('');
    this.store.setCategoriaFilter(null);
    this.store.setSoloStockBajo(false);
  }

  onDelete(producto: Producto): void {
    if (confirm(`¿Está seguro de eliminar el producto "${producto.nombre}"?`)) {
      this.store.deleteProducto(producto.id);
    }
  }
}
