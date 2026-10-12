import { HttpClient } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { Categoria, CreateCategoriaDto, UpdateCategoriaDto } from '../models/categoria.model';

@Injectable({
  providedIn: 'root'
})
export class CategoriasService {
  private readonly http = inject(HttpClient);
  private readonly baseUrl = 'http://localhost:5000/api/categorias';

  getCategorias(): Observable<Categoria[]> {
    return this.http.get<Categoria[]>(this.baseUrl);
  }

  createCategoria(dto: CreateCategoriaDto): Observable<Categoria> {
    return this.http.post<Categoria>(this.baseUrl, dto);
  }

  updateCategoria(id: number, dto: UpdateCategoriaDto): Observable<Categoria> {
    return this.http.put<Categoria>(`${this.baseUrl}/${id}`, dto);
  }

  deleteCategoria(id: number): Observable<void> {
    return this.http.delete<void>(`${this.baseUrl}/${id}`);
  }
}
