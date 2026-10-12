import { HttpClient } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { Categoria } from '../models/categoria.model';
import { MovimientoInventario } from '../models/movimiento-inventario.model';
import { AjustarStockRequest, CrearProductoRequest, EditarProductoRequest, Producto } from '../models/producto.model';

@Injectable({
  providedIn: 'root'
})
export class ProductosService {
  private readonly http = inject(HttpClient);
  private readonly baseUrl = 'http://localhost:5000/api';

  getProductos(): Observable<Producto[]> {
    return this.http.get<Producto[]>(`${this.baseUrl}/productos`);
  }

  getProductosStockBajo(): Observable<Producto[]> {
    return this.http.get<Producto[]>(`${this.baseUrl}/productos/stock-bajo`);
  }

  getProductoById(id: number): Observable<Producto> {
    return this.http.get<Producto>(`${this.baseUrl}/productos/${id}`);
  }

  getMovimientosByProductoId(id: number): Observable<MovimientoInventario[]> {
    return this.http.get<MovimientoInventario[]>(`${this.baseUrl}/productos/${id}/movimientos`);
  }

  createProducto(payload: CrearProductoRequest): Observable<Producto> {
    return this.http.post<Producto>(`${this.baseUrl}/productos`, payload);
  }

  updateProducto(id: number, payload: EditarProductoRequest): Observable<Producto> {
    return this.http.put<Producto>(`${this.baseUrl}/productos/${id}`, payload);
  }

  adjustStock(id: number, payload: AjustarStockRequest): Observable<Producto> {
    return this.http.patch<Producto>(`${this.baseUrl}/productos/${id}/stock`, payload);
  }

  deleteProducto(id: number): Observable<void> {
    return this.http.delete<void>(`${this.baseUrl}/productos/${id}`);
  }

  getCategorias(): Observable<Categoria[]> {
    return this.http.get<Categoria[]>(`${this.baseUrl}/categorias`);
  }
}
