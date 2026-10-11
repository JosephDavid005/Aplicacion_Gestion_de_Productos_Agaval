using System.Reflection;
using FluentAssertions;
using Xunit;

namespace GestorInventario.Architecture.Tests;

public class LayerDependencyTests
{
    private static readonly Assembly Domain = GestorInventario.Domain.AssemblyReference.Assembly;
    private static readonly Assembly Application = GestorInventario.Application.AssemblyReference.Assembly;
    private static readonly Assembly Infrastructure = GestorInventario.Infrastructure.AssemblyReference.Assembly;
    private static readonly Assembly Api = GestorInventario.Api.AssemblyReference.Assembly;

    private static string[] ProjectReferences(Assembly assembly) =>
        assembly.GetReferencedAssemblies()
            .Select(a => a.Name!)
            .Where(n => n.StartsWith("GestorInventario.", StringComparison.Ordinal))
            .ToArray();

    [Fact]
    public void Domain_ShouldNotDependOnAnyOtherLayer()
    {
        ProjectReferences(Domain).Should().BeEmpty();
    }

    [Fact]
    public void Application_ShouldOnlyDependOnDomain()
    {
        ProjectReferences(Application).Should().BeSubsetOf(new[] { "GestorInventario.Domain" });
    }

    [Fact]
    public void Infrastructure_ShouldNotDependOnApi()
    {
        ProjectReferences(Infrastructure).Should().NotContain("GestorInventario.Api");
    }

    [Fact]
    public void Application_ShouldNotDependOnInfrastructureOrApi()
    {
        ProjectReferences(Application).Should()
            .NotContain(new[] { "GestorInventario.Infrastructure", "GestorInventario.Api" });
    }

    [Fact]
    public void Api_ShouldNotDependOnDomainEntitiesDirectlyViaPersistence()
    {
        Api.GetReferencedAssemblies().Select(a => a.Name)
            .Should().NotContain("Microsoft.EntityFrameworkCore");
    }
}
