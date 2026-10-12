using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using FluentAssertions;
using GestorInventario.Application.Categorias.Commands.CrearCategoria;
using GestorInventario.Application.Categorias.Commands.EditarCategoria;
using GestorInventario.Application.Categorias.Common;
using Xunit;

namespace GestorInventario.Api.IntegrationTests;

public class CategoriasEndpointsTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client;
    private static readonly JsonSerializerOptions JsonOptions = new() { PropertyNameCaseInsensitive = true };

    public CategoriasEndpointsTests(CustomWebApplicationFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task PostCategoria_WhenValid_ShouldReturn201Created()
    {
        var command = new CrearCategoriaCommand("Jardinería", true);

        var response = await _client.PostAsJsonAsync("/api/categorias", command);

        response.StatusCode.Should().Be(HttpStatusCode.Created);
        var created = await response.Content.ReadFromJsonAsync<CategoriaDto>(JsonOptions);
        created.Should().NotBeNull();
        created!.Nombre.Should().Be("Jardinería");
        created.Activo.Should().BeTrue();
    }

    [Fact]
    public async Task PostCategoria_WhenDuplicateName_ShouldReturn400BadRequest()
    {
        // "Electrónica" ya existe en el seed
        var command = new CrearCategoriaCommand("Electrónica", true);

        var response = await _client.PostAsJsonAsync("/api/categorias", command);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task PutCategoria_WhenValid_ShouldReturn200Ok()
    {
        // Creamos una categoría para editar
        var createResponse = await _client.PostAsJsonAsync("/api/categorias", new CrearCategoriaCommand("Deportes V1", true));
        createResponse.StatusCode.Should().Be(HttpStatusCode.Created);
        var created = await createResponse.Content.ReadFromJsonAsync<CategoriaDto>(JsonOptions);

        var updateCommand = new { Nombre = "Deportes V2", Activo = false };
        var putResponse = await _client.PutAsJsonAsync($"/api/categorias/{created!.Id}", updateCommand);

        putResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        var updated = await putResponse.Content.ReadFromJsonAsync<CategoriaDto>(JsonOptions);
        updated.Should().NotBeNull();
        updated!.Nombre.Should().Be("Deportes V2");
        updated.Activo.Should().BeFalse();
    }

    [Fact]
    public async Task PutCategoria_WhenNotFound_ShouldReturn404NotFound()
    {
        var updateCommand = new { Nombre = "No Existe", Activo = true };
        var response = await _client.PutAsJsonAsync("/api/categorias/99999", updateCommand);

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task DeleteCategoria_WhenHasProducts_ShouldReturn400BadRequest()
    {
        // Categoría 1 ("Electrónica") tiene productos asociados del DataSeeder
        var response = await _client.DeleteAsync("/api/categorias/1");

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task DeleteCategoria_WhenNoProducts_ShouldReturn204NoContent()
    {
        // Creamos una categoría vacía
        var createResponse = await _client.PostAsJsonAsync("/api/categorias", new CrearCategoriaCommand("Borrable", true));
        var created = await createResponse.Content.ReadFromJsonAsync<CategoriaDto>(JsonOptions);

        var deleteResponse = await _client.DeleteAsync($"/api/categorias/{created!.Id}");

        deleteResponse.StatusCode.Should().Be(HttpStatusCode.NoContent);
    }
}
