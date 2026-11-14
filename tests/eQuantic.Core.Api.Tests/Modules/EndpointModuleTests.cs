using System.Text.Json;
using eQuantic.Core.Api.Extensions;
using eQuantic.Core.Api.Modules;
using FluentAssertions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;

namespace eQuantic.Core.Api.Tests.Modules;

public sealed class OrdersModule : IEndpointModule
{
    public void MapEndpoints(IEndpointRouteBuilder endpoints) => endpoints.MapGet("/orders", () => "orders");
}

public abstract class ReportsModuleBase : IEndpointModule
{
    public abstract void MapEndpoints(IEndpointRouteBuilder endpoints);
}

public sealed class SalesReportModule : ReportsModuleBase
{
    public override void MapEndpoints(IEndpointRouteBuilder endpoints) => endpoints.MapGet("/reports/sales", () => "sales");
}

public sealed class GenericModule<T> : IEndpointModule
{
    public void MapEndpoints(IEndpointRouteBuilder endpoints) => endpoints.MapGet($"/generic/{typeof(T).Name}", () => "generic");
}

public sealed class Outer<T>
{
    public sealed class InnerModule : IEndpointModule
    {
        public void MapEndpoints(IEndpointRouteBuilder endpoints) => endpoints.MapGet($"/inner/{typeof(T).Name}", () => "inner");
    }
}

public class EndpointModuleTests
{
    [Test]
    public void AddEndpointModules_registers_every_concrete_module_of_the_assembly()
    {
        var services = new ServiceCollection();

        services.AddEndpointModules(typeof(OrdersModule).Assembly);

        using var provider = services.BuildServiceProvider();
        provider.GetServices<IEndpointModule>().Select(m => m.GetType())
            .Should().BeEquivalentTo([typeof(OrdersModule), typeof(SalesReportModule)]);
    }

    [Test]
    public void AddEndpointModules_registers_nothing_twice()
    {
        var services = new ServiceCollection();
        var assembly = typeof(OrdersModule).Assembly;

        services.AddEndpointModules(assembly, assembly);
        services.AddEndpointModules(assembly);

        services.Where(d => d.ServiceType == typeof(IEndpointModule)).Should().HaveCount(2);
    }

    [Test]
    public void AddEndpointModules_refuses_a_null_assembly()
    {
        var services = new ServiceCollection();

        var act = () => services.AddEndpointModules(typeof(OrdersModule).Assembly, null!);

        act.Should().Throw<ArgumentNullException>();
    }

    [Test]
    public async Task MapModules_answers_on_the_routes_the_registered_modules_map()
    {
        var builder = WebApplication.CreateBuilder();
        builder.WebHost.UseTestServer();
        builder.Services.AddEndpointModules(typeof(OrdersModule).Assembly);

        await using var app = builder.Build();
        app.MapModules();
        await app.StartAsync();

        using var client = app.GetTestClient();
        (await client.GetStringAsync("/orders")).Should().Be("orders");
        (await client.GetStringAsync("/reports/sales")).Should().Be("sales");
        (await client.GetAsync("/generic/String")).StatusCode.Should().Be(System.Net.HttpStatusCode.NotFound);

        await app.StopAsync();
    }

    /// <summary>
    /// eQuantic.Core.Api registers through Microsoft.Extensions.DependencyInjection only. This reads the test
    /// project's resolved dependency closure, which is the package's own (nothing else here could bring
    /// Lamar), so a package reference that compiles to nothing is caught as well.
    /// </summary>
    [Test]
    public void The_package_brings_no_lamar()
    {
        var assembly = typeof(EndpointModuleTests).Assembly.GetName().Name;
        var path = Path.Combine(AppContext.BaseDirectory, $"{assembly}.deps.json");
        File.Exists(path).Should().BeTrue($"the SDK writes {path} beside the test assembly");

        using var document = JsonDocument.Parse(File.ReadAllText(path));
        var libraries = document.RootElement.GetProperty("libraries").EnumerateObject().Select(p => p.Name).ToList();

        // The closure was really read: the package and what it stands on are in it.
        libraries.Should().Contain(l => l.StartsWith("eQuantic.Core.Api/", StringComparison.Ordinal));
        libraries.Should().Contain(l => l.StartsWith("eQuantic.Core.Application/", StringComparison.Ordinal));

        libraries.Where(l =>
                l.StartsWith("Lamar", StringComparison.OrdinalIgnoreCase)
                || l.StartsWith("JasperFx", StringComparison.OrdinalIgnoreCase))
            .Should().BeEmpty("eQuantic.Core.Api must not bring an IoC container to the applications built on it");
    }
}
