using System.Text.Json;
using System.Text.Json.Serialization;
using eQuantic.Core.Api.Extensions;
using eQuantic.Core.Api.Sample;
using eQuantic.Core.Api.Sample.Entities;
using eQuantic.Core.Api.Sample.Services;
using eQuantic.Core.Application;
using eQuantic.Core.Data.EntityFramework.Repository.Extensions;
using eQuantic.Core.Mvc.Extensions;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);
var assembly = typeof(Program).Assembly;

builder.Services.AddDbContext<ExampleDbContext>(opt =>
    opt.UseInMemoryDatabase("ExampleDb"));
        
builder.Services.AddQueryableRepositories<ExampleUnitOfWork>(opt =>
{
    opt.FromAssembly(assembly)
        .AddLifetime(ServiceLifetime.Scoped);
});

builder.Services
    .AddMappers(opt => opt.FromAssembly(assembly))
    .AddHttpContextAccessor()
    .AddTransient<IApplicationContext<int>, ApplicationContext>()
    .AddTransient<IExampleService, ExampleService>()
    .AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
        options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
    })
    .AddFilterModelBinder()
    .AddSortModelBinder();

builder.Services
    .AddApiDocumentation(opt => opt.WithTitle("Example API"));

var app = builder.Build();
if (app.Environment.IsDevelopment())
{
    app.UseApiDocumentation(opt => opt.WithSignIn("http://localhost:5153/login"));
}

app.UseHttpsRedirection();
app.UseRouting();
app.MapControllers();
app.MapGet("examples/{id}", GetById)
    .WithTags("Example")
    .Produces<Example>(StatusCodes.Status200OK);

app.Run();

return;

async Task<Results<Ok<Example>, NotFound>> GetById(
    [FromRoute] int id, 
    [FromServices] IExampleService service, 
    CancellationToken cancellationToken)
{
    var item = await service.GetByIdAsync(id, cancellationToken);
    if (item == null)
        return TypedResults.NotFound();

    return TypedResults.Ok(item);
}