using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using FluentAssertions;
using GestorInventario.Api.Controllers;
using GestorInventario.Application.Categorias.Common;
using GestorInventario.Application.Productos.Commands.AjustarStock;
using GestorInventario.Application.Productos.Commands.CrearProducto;
using GestorInventario.Application.Productos.Commands.EditarProducto;
using GestorInventario.Application.Productos.Common;
using GestorInventario.Domain.Enums;
using Xunit;

namespace GestorInventario.Api.IntegrationTests;

public class ProductosEndpointsTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client;
    private static readonly JsonSerializerOptions JsonOptions = new() { PropertyNameCaseInsensitive = true };

    public ProductosEndpointsTests(CustomWebApplicationFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task SwaggerEndpoint_ShouldReturn200Ok()
    {
        var response = await _client.GetAsync("/swagger/v1/swagger.json");
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task GetCategorias_ShouldReturnSeededCategories()
    {
        var response = await _client.GetAsync("/api/categorias");
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var categorias = await response.Content.ReadFromJsonAsync<List<CategoriaDto>>(JsonOptions);
        categorias.Should().NotBeNull();
        categorias!.Count.Should().BeGreaterThanOrEqualTo(3);
        categorias.Select(c => c.Nombre).Should().Contain(new[] { "Electrónica", "Oficina", "Aseo" });
    }

    [Fact]
    public async Task GetProductos_ShouldReturnAllProducts()
    {
        var response = await _client.GetAsync("/api/productos");
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var productos = await response.Content.ReadFromJsonAsync<List<ProductoDto>>(JsonOptions);
        productos.Should().NotBeNull();
        productos!.Should().NotBeEmpty();
    }

    [Fact]
    public async Task GetProductosStockBajo_ShouldOnlyReturnLowStockProducts()
    {
        var response = await _client.GetAsync("/api/productos/stock-bajo");
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var productos = await response.Content.ReadFromJsonAsync<List<ProductoDto>>(JsonOptions);
        productos.Should().NotBeNull();
        productos!.Should().NotBeEmpty();
        productos.Should().OnlyContain(p => p.IsLowStock);
    }

    [Fact]
    public async Task CreateProducto_WithValidData_ShouldReturn201Created()
    {
        var command = new CrearProductoCommand("Audífonos Bluetooth", "Cancelación de ruido", 180000m, 15, 5, 1);

        var response = await _client.PostAsJsonAsync("/api/productos", command);

        response.StatusCode.Should().Be(HttpStatusCode.Created);
        response.Headers.Location.Should().NotBeNull();

        var created = await response.Content.ReadFromJsonAsync<ProductoDto>(JsonOptions);
        created.Should().NotBeNull();
        created!.Nombre.Should().Be("Audífonos Bluetooth");
        created.Precio.Should().Be(180000m);
    }

    [Fact]
    public async Task CreateProducto_WithInvalidData_ShouldReturn400BadRequest()
    {
        // Nombre vacío y precio negativo
        var command = new CrearProductoCommand("", "Desc", -500m, 10, 5, 1);

        var response = await _client.PostAsJsonAsync("/api/productos", command);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task GetProductoById_WhenDoesNotExist_ShouldReturn404NotFound()
    {
        var response = await _client.GetAsync("/api/productos/99999");
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task AdjustStock_WithEntradaAndSalida_ShouldUpdateStockCorrectly()
    {
        // 1. Crear producto con stock 10
        var createCmd = new CrearProductoCommand("Cable HDMI 2.1", "4K 120Hz", 35000m, 10, 5, 1);
        var createRes = await _client.PostAsJsonAsync("/api/productos", createCmd);
        var prod = await createRes.Content.ReadFromJsonAsync<ProductoDto>(JsonOptions);

        // 2. Ajustar entrada (+5)
        var entradaCmd = new AjustarStockCommand(prod!.Id, TipoMovimiento.Entrada, 5);
        var entradaRes = await _client.PatchAsJsonAsync($"/api/productos/{prod.Id}/stock", entradaCmd);
        entradaRes.StatusCode.Should().Be(HttpStatusCode.OK);
        var prodEntrada = await entradaRes.Content.ReadFromJsonAsync<ProductoDto>(JsonOptions);
        prodEntrada!.Stock.Should().Be(15);

        // 3. Ajustar salida (-7)
        var salidaCmd = new AjustarStockCommand(prod.Id, TipoMovimiento.Salida, 7);
        var salidaRes = await _client.PatchAsJsonAsync($"/api/productos/{prod.Id}/stock", salidaCmd);
        salidaRes.StatusCode.Should().Be(HttpStatusCode.OK);
        var prodSalida = await salidaRes.Content.ReadFromJsonAsync<ProductoDto>(JsonOptions);
        prodSalida!.Stock.Should().Be(8);

        // 4. Salida excesiva (-10 cuando quedan 8) -> 400 Bad Request
        var salidaExcesiva = new AjustarStockCommand(prod.Id, TipoMovimiento.Salida, 10);
        var errRes = await _client.PatchAsJsonAsync($"/api/productos/{prod.Id}/stock", salidaExcesiva);
        errRes.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task DeleteProducto_WhenExists_ShouldReturn204NoContent()
    {
        // 1. Crear producto para borrar
        var createCmd = new CrearProductoCommand("Producto Temporal", "Para borrar", 1000m, 1, 1, 1);
        var createRes = await _client.PostAsJsonAsync("/api/productos", createCmd);
        var prod = await createRes.Content.ReadFromJsonAsync<ProductoDto>(JsonOptions);

        // 2. Eliminar
        var deleteRes = await _client.DeleteAsync($"/api/productos/{prod!.Id}");
        deleteRes.StatusCode.Should().Be(HttpStatusCode.NoContent);

        // 3. Verificar que ya no existe -> 404
        var getRes = await _client.GetAsync($"/api/productos/{prod.Id}");
        getRes.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task GetMovimientos_WhenProductExists_ShouldReturn200AndAuditHistory()
    {
        // 1. Crear producto
        var createCmd = new CrearProductoCommand("Audited Product", "Para probar kardex", 50000m, 10, 2, 1);
        var createRes = await _client.PostAsJsonAsync("/api/productos", createCmd);
        var prod = await createRes.Content.ReadFromJsonAsync<ProductoDto>(JsonOptions);

        // 2. Ajustar stock (Entrada y Salida)
        var entrada = new AjusteStockRequest(TipoMovimiento.Entrada, 5, "Lote Auditoría A");
        await _client.PatchAsJsonAsync($"/api/productos/{prod!.Id}/stock", entrada);

        var salida = new AjusteStockRequest(TipoMovimiento.Salida, 3, "Venta Auditoría B");
        await _client.PatchAsJsonAsync($"/api/productos/{prod.Id}/stock", salida);

        // 3. Consultar movimientos
        var movRes = await _client.GetAsync($"/api/productos/{prod.Id}/movimientos");
        movRes.StatusCode.Should().Be(HttpStatusCode.OK);

        var movimientos = await movRes.Content.ReadFromJsonAsync<List<GestorInventario.Application.Productos.Queries.ObtenerMovimientos.MovimientoInventarioDto>>(JsonOptions);
        movimientos.Should().NotBeNull();
        movimientos!.Should().HaveCount(2);
        movimientos[0].TipoMovimiento.Should().Be("SALIDA");
        movimientos[0].Cantidad.Should().Be(3);
        movimientos[0].Observacion.Should().Be("Venta Auditoría B");
        movimientos[1].TipoMovimiento.Should().Be("ENTRADA");
        movimientos[1].Cantidad.Should().Be(5);
        movimientos[1].Observacion.Should().Be("Lote Auditoría A");
    }

    [Fact]
    public async Task GetMovimientos_WhenProductDoesNotExist_ShouldReturn404NotFound()
    {
        var res = await _client.GetAsync("/api/productos/99999/movimientos");
        res.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
}
