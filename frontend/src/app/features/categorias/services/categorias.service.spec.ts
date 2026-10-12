import { TestBed } from '@angular/core/testing';
import { provideHttpClient } from '@angular/common/http';
import { HttpTestingController, provideHttpClientTesting } from '@angular/common/http/testing';
import { CategoriasService } from './categorias.service';
import { Categoria, CreateCategoriaDto, UpdateCategoriaDto } from '../models/categoria.model';

describe('CategoriasService', () => {
  let service: CategoriasService;
  let httpTesting: HttpTestingController;
  const baseUrl = 'http://localhost:5000/api/categorias';

  beforeEach(() => {
    TestBed.configureTestingModule({
      providers: [
        CategoriasService,
        provideHttpClient(),
        provideHttpClientTesting()
      ]
    });

    service = TestBed.inject(CategoriasService);
    httpTesting = TestBed.inject(HttpTestingController);
  });

  afterEach(() => {
    httpTesting.verify();
  });

  it('getCategorias debe enviar GET a /categorias y retornar listado', () => {
    const mockCategorias: Categoria[] = [
      { id: 1, nombre: 'Electrónica', activo: true },
      { id: 2, nombre: 'Oficina', activo: true }
    ];

    service.getCategorias().subscribe((cats: Categoria[]) => {
      expect(cats).toEqual(mockCategorias);
    });

    const req = httpTesting.expectOne(baseUrl);
    expect(req.request.method).toBe('GET');
    req.flush(mockCategorias);
  });

  it('createCategoria debe enviar POST a /categorias con el DTO', () => {
    const newDto: CreateCategoriaDto = { nombre: 'Jardinería', activo: true };
    const createdCat: Categoria = { id: 3, nombre: 'Jardinería', activo: true };

    service.createCategoria(newDto).subscribe((result: Categoria) => {
      expect(result).toEqual(createdCat);
    });

    const req = httpTesting.expectOne(baseUrl);
    expect(req.request.method).toBe('POST');
    expect(req.request.body).toEqual(newDto);
    req.flush(createdCat);
  });

  it('updateCategoria debe enviar PUT a /categorias/:id con el DTO', () => {
    const updateDto: UpdateCategoriaDto = { nombre: 'Electrónica Pro', activo: true };
    const updatedCat: Categoria = { id: 1, nombre: 'Electrónica Pro', activo: true };

    service.updateCategoria(1, updateDto).subscribe((result: Categoria) => {
      expect(result).toEqual(updatedCat);
    });

    const req = httpTesting.expectOne(`${baseUrl}/1`);
    expect(req.request.method).toBe('PUT');
    expect(req.request.body).toEqual(updateDto);
    req.flush(updatedCat);
  });

  it('deleteCategoria debe enviar DELETE a /categorias/:id', () => {
    service.deleteCategoria(1).subscribe();

    const req = httpTesting.expectOne(`${baseUrl}/1`);
    expect(req.request.method).toBe('DELETE');
    req.flush(null);
  });
});
