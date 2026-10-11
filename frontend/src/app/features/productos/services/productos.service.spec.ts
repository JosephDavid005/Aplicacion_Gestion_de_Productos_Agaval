import { provideHttpClient } from '@angular/common/http';
import { HttpTestingController, provideHttpClientTesting } from '@angular/common/http/testing';
import { TestBed } from '@angular/core/testing';
import { Categoria } from '../models/categoria.model';
import { AjustarStockRequest, CrearProductoRequest, EditarProductoRequest, Producto } from '../models/producto.model';
import { ProductosService } from './productos.service';

describe('ProductosService', () => {
  let service: ProductosService;
  let httpTesting: HttpTestingController;
  const baseUrl = 'http://localhost:5000/api';

  beforeEach(() => {
    TestBed.configureTestingModule({
      providers: [
        ProductosService,
        provideHttpClient(),
        provideHttpClientTesting()
      ]
    });

    service = TestBed.inject(ProductosService);
    httpTesting = TestBed.inject(HttpTestingController);
  });

  afterEach(() => {
    httpTesting.verify();
  });

  it('debe crearse correctamente', () => {
    expect(service).toBeTruthy();
  });

  it('getProductos debe enviar GET y retornar lista de productos', () => {
    const mockProductos: Producto[] = [
      {
        id: 1,
        nombre: 'Laptop',
        descripcion: '16GB',
        precio: 3000000,
        stock: 10,
        stockMinimo: 5,
        categoriaId: 1,
        categoriaNombre: 'Electrónica',
        isLowStock: false,
        fechaCreacion: '2026-10-10'
      }
    ];

    service.getProductos().subscribe(prods => {
      expect(prods).toEqual(mockProductos);
      expect(prods.length).toBe(1);
    });

    const req = httpTesting.expectOne(`${baseUrl}/productos`);
    expect(req.request.method).toBe('GET');
    req.flush(mockProductos);
  });

  it('getProductosStockBajo debe enviar GET a /productos/stock-bajo', () => {
    service.getProductosStockBajo().subscribe();

    const req = httpTesting.expectOne(`${baseUrl}/productos/stock-bajo`);
    expect(req.request.method).toBe('GET');
    req.flush([]);
  });

  it('createProducto debe enviar POST con el payload', () => {
    const payload: CrearProductoRequest = {
      nombre: 'Mouse',
      descripcion: 'Óptico',
      precio: 50000,
      stock: 4,
      stockMinimo: 5,
      categoriaId: 1
    };

    const mockResponse: Producto = {
      ...payload,
      id: 2,
      categoriaNombre: 'Electrónica',
      isLowStock: true,
      fechaCreacion: '2026-10-10'
    };

    service.createProducto(payload).subscribe(res => {
      expect(res.id).toBe(2);
      expect(res.isLowStock).toBe(true);
    });

    const req = httpTesting.expectOne(`${baseUrl}/productos`);
    expect(req.request.method).toBe('POST');
    expect(req.request.body).toEqual(payload);
    req.flush(mockResponse);
  });

  it('adjustStock debe enviar PATCH con el ajuste a /productos/{id}/stock', () => {
    const payload: AjustarStockRequest = {
      tipo: 1,
      cantidad: 5
    };

    service.adjustStock(10, payload).subscribe();

    const req = httpTesting.expectOne(`${baseUrl}/productos/10/stock`);
    expect(req.request.method).toBe('PATCH');
    expect(req.request.body).toEqual(payload);
    req.flush({ id: 10, stock: 15 });
  });

  it('getCategorias debe enviar GET a /categorias', () => {
    const mockCategorias: Categoria[] = [
      { id: 1, nombre: 'Electrónica', activo: true }
    ];

    service.getCategorias().subscribe(cats => {
      expect(cats).toEqual(mockCategorias);
    });

    const req = httpTesting.expectOne(`${baseUrl}/categorias`);
    expect(req.request.method).toBe('GET');
    req.flush(mockCategorias);
  });
});

