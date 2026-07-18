using eQuantic.Core.Domain.Entities;
using eQuantic.Core.Domain.Entities.Requests;
using FluentAssertions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace eQuantic.Core.Api.Tests.Binding;

public class ProbeEntity
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public decimal Total { get; set; }
}

[ApiController]
public class BindingProbeController : ControllerBase
{
    [HttpGet("/mvc-envelope")]
    public IActionResult Get([FromQuery] PagedListRequest<ProbeEntity> request) =>
        Ok(new
        {
            filters = request.FilterBy?.Count ?? 0,
            sorts = request.OrderBy?.Count ?? 0,
            predicate = request.GetFilterPredicate()?.ToString() ?? "none",
        });
}

/// <summary>
/// Empirical proof of the v3 binding story: the typed collections must bind from the query
/// string in BOTH pipelines without any external model binder package.
/// </summary>
public class QueryCollectionBindingTests
{
    private static async Task<HttpClient> BuildAppAsync()
    {
        var builder = WebApplication.CreateBuilder();
        builder.WebHost.UseTestServer();
        builder.Logging.ClearProviders();
        builder.Services.AddControllers().AddApplicationPart(typeof(BindingProbeController).Assembly);

        var app = builder.Build();
        app.MapControllers();
        app.MapGet("/minimal-envelope", ([AsParameters] PagedListRequest<ProbeEntity> request) =>
            Results.Ok(new
            {
                filters = request.FilterBy?.Count ?? 0,
                sorts = request.OrderBy?.Count ?? 0,
            }));

        await app.StartAsync();
        return app.GetTestClient();
    }

    [Test]
    public async Task Minimal_api_envelope_binds_filter_and_sort_collections()
    {
        var client = await BuildAppAsync();

        var response = await client.GetAsync("/minimal-envelope?filterBy=total:gt(100)&orderBy=name:desc");
        var body = await response.Content.ReadAsStringAsync();

        response.IsSuccessStatusCode.Should().BeTrue(body);
        body.Should().Contain("\"filters\":1").And.Contain("\"sorts\":1");
    }

    [Test]
    public async Task Mvc_envelope_binds_filter_and_sort_collections()
    {
        var client = await BuildAppAsync();

        var response = await client.GetAsync("/mvc-envelope?filterBy=total:gt(100)&orderBy=name:desc");
        var body = await response.Content.ReadAsStringAsync();

        response.IsSuccessStatusCode.Should().BeTrue(body);
        body.Should().Contain("\"filters\":1").And.Contain("\"sorts\":1");
        body.Should().Contain("Total").And.Contain("100");
    }
}
