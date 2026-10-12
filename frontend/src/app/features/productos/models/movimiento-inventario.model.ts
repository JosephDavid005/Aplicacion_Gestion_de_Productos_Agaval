export interface MovimientoInventario {
  id: number;
  productoId: number;
  tipoMovimiento: 'ENTRADA' | 'SALIDA';
  cantidad: number;
  fecha: string;
  observacion: string | null;
}
