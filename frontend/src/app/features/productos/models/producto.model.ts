export interface Producto {
  id: number;
  nombre: string;
  descripcion: string | null;
  precio: number;
  stock: number;
  stockMinimo: number;
  categoriaId: number;
  categoriaNombre?: string | null;
  isLowStock: boolean;
  fechaCreacion: string;
}

export interface CrearProductoRequest {
  nombre: string;
  descripcion: string | null;
  precio: number;
  stock: number;
  stockMinimo: number;
  categoriaId: number;
}

export interface EditarProductoRequest {
  nombre: string;
  descripcion: string | null;
  precio: number;
  stockMinimo: number;
  categoriaId: number;
}

export type TipoMovimiento = 1 | 2; // 1 = Entrada, 2 = Salida

export interface AjustarStockRequest {
  tipo: TipoMovimiento;
  cantidad: number;
}
