import { ChangeDetectionStrategy, Component, inject, OnInit, signal } from '@angular/core';
import { DatePipe } from '@angular/common';
import { MAT_DIALOG_DATA, MatDialogModule, MatDialogRef } from '@angular/material/dialog';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatTableModule } from '@angular/material/table';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MovimientoInventario } from '../../models/movimiento-inventario.model';
import { Producto } from '../../models/producto.model';
import { ProductosService } from '../../services/productos.service';

export interface MovimientosHistoricoDialogData {
  producto: Producto;
}

@Component({
  selector: 'app-movimientos-historico-dialog',
  standalone: true,
  imports: [
    DatePipe,
    MatDialogModule,
    MatButtonModule,
    MatIconModule,
    MatTableModule,
    MatProgressSpinnerModule
  ],
  templateUrl: './movimientos-historico-dialog.component.html',
  styleUrl: './movimientos-historico-dialog.component.scss',
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class MovimientosHistoricoDialogComponent implements OnInit {
  readonly dialogRef = inject(MatDialogRef<MovimientosHistoricoDialogComponent>);
  readonly data: MovimientosHistoricoDialogData = inject(MAT_DIALOG_DATA);
  private readonly productosService = inject(ProductosService);

  readonly producto: Producto = this.data.producto;
  readonly movimientos = signal<MovimientoInventario[]>([]);
  readonly isLoading = signal<boolean>(true);
  readonly errorMessage = signal<string | null>(null);

  readonly displayedColumns: string[] = ['fecha', 'tipo', 'cantidad', 'observacion'];

  ngOnInit(): void {
    this.cargarMovimientos();
  }

  cargarMovimientos(): void {
    this.isLoading.set(true);
    this.errorMessage.set(null);

    this.productosService.getMovimientosByProductoId(this.producto.id).subscribe({
      next: (data: MovimientoInventario[]) => {
        this.movimientos.set(data);
        this.isLoading.set(false);
      },
      error: (err: unknown) => {
        this.errorMessage.set(err instanceof Error ? err.message : 'Error al cargar historial');
        this.isLoading.set(false);
      }
    });
  }

  onClose(): void {
    this.dialogRef.close();
  }
}
