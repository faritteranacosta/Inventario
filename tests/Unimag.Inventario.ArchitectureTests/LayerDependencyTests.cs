using System.Reflection;
using NetArchTest.Rules;

namespace Unimag.Inventario.ArchitectureTests;

/// <summary>
/// Enforces the dependency-direction rules from the root CLAUDE.md (section
/// "Regla de oro: las flechas de dependencia solo apuntan hacia adentro").
/// These are assembly-level checks — they fail the build if someone adds a
/// ProjectReference or writes code that points a layer outward, regardless
/// of whether the offending code is ever exercised by another test.
/// </summary>
public class LayerDependencyTests
{
    private static readonly Assembly DomainAssembly = typeof(Unimag.Inventario.Domain.Common.BaseEntity).Assembly;
    private static readonly Assembly ApplicationAssembly = typeof(Unimag.Inventario.Application.DependencyInjection).Assembly;
    private static readonly Assembly InfrastructureAssembly = typeof(Unimag.Inventario.Infrastructure.DependencyInjection).Assembly;

    private const string DomainNamespace = "Unimag.Inventario.Domain";
    private const string ApplicationNamespace = "Unimag.Inventario.Application";
    private const string InfrastructureNamespace = "Unimag.Inventario.Infrastructure";
    private const string ApiNamespace = "Unimag.Inventario.Api";
    private const string EntityFrameworkCoreNamespace = "Microsoft.EntityFrameworkCore";
    private const string AspNetCoreNamespace = "Microsoft.AspNetCore";

    [Fact]
    public void Domain_Should_Not_HaveDependencyOnOtherLayers()
    {
        var result = Types.InAssembly(DomainAssembly)
            .Should()
            .NotHaveDependencyOnAny(ApplicationNamespace, InfrastructureNamespace, ApiNamespace)
            .GetResult();

        Assert.True(result.IsSuccessful, Describe(result));
    }

    [Fact]
    public void Domain_Should_Not_HaveDependencyOnFrameworks()
    {
        // Domain has zero NuGet dependencies — if this ever fails, someone
        // added EF Core/ASP.NET Core (directly or transitively) to Domain.
        var result = Types.InAssembly(DomainAssembly)
            .Should()
            .NotHaveDependencyOnAny(EntityFrameworkCoreNamespace, AspNetCoreNamespace)
            .GetResult();

        Assert.True(result.IsSuccessful, Describe(result));
    }

    [Fact]
    public void Application_Should_Not_HaveDependencyOnOuterLayers()
    {
        var result = Types.InAssembly(ApplicationAssembly)
            .Should()
            .NotHaveDependencyOnAny(InfrastructureNamespace, ApiNamespace)
            .GetResult();

        Assert.True(result.IsSuccessful, Describe(result));
    }

    [Fact]
    public void Application_Should_Not_HaveDependencyOnFrameworks()
    {
        // A use case must not know about EF Core or ASP.NET Core — see
        // CLAUDE.md section 3 ("Nunca de Microsoft.AspNetCore.* ni de
        // Microsoft.EntityFrameworkCore.*").
        var result = Types.InAssembly(ApplicationAssembly)
            .Should()
            .NotHaveDependencyOnAny(EntityFrameworkCoreNamespace, AspNetCoreNamespace)
            .GetResult();

        Assert.True(result.IsSuccessful, Describe(result));
    }

    [Fact]
    public void Infrastructure_Should_Not_HaveDependencyOnApi()
    {
        var result = Types.InAssembly(InfrastructureAssembly)
            .Should()
            .NotHaveDependencyOn(ApiNamespace)
            .GetResult();

        Assert.True(result.IsSuccessful, Describe(result));
    }

    private static string Describe(TestResult result)
    {
        if (result.IsSuccessful || result.FailingTypes is null)
        {
            return string.Empty;
        }

        var offenders = result.FailingTypeNames ?? result.FailingTypes.Select(t => t.FullName ?? t.Name);
        return "Violating types: " + string.Join(", ", offenders);
    }
}
