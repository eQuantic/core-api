# eQuantic.Core.Api Library

The **eQuantic Core API** provides all the implementation needed to publish standard APIs.

To install **eQuantic.Core.Api**, run the following command in the [Package Manager Console](https://docs.nuget.org/docs/start-here/using-the-package-manager-console)
```dos
Install-Package eQuantic.Core.Api
```

## Example of implementation

### The data entities
```csharp
[Table("orders")]
public class OrderData : EntityDataBase
{
    [Key]
    public string Id { get; set; } = string.Empty;
    public DateTime Date { get; set; }
    
    public virtual ICollection<OrderItemData> Items { get; set; } = new HashSet<OrderItemData>();
}

[Table("orderItems")]
public class OrderItemData : EntityDataBase, IWithReferenceId<OrderItemData, int>
{
    [Key]
    public int Id { get; set; }
    public int OrderId { get; set; }
    
    [ForeignKey(nameof(OrderId))]
    public virtual OrderData? Order { get; set; }
    
    [Required]
    [MaxLength(200)]
    public string Name { get; set; } = string.Empty;
}
```

### The models
```csharp
public class Order
{
    public string Id { get; set; } = string.Empty;
    public DateTime Date { get; set; }
}

public class OrderItem
{
    public int Id { get; set; }
    public int OrderId { get; set; }
    public string Name { get; set; } = string.Empty;
}
```

### The request models
```csharp
public class OrderRequest
{
    public DateTime? Date { get; set; }
}

public class OrderItemRequest
{
    public string? Name { get; set; }
}
```
### The mappers

```csharp
public class OrderMapper : IMapper<OrderData, Order>, IMapper<OrderRequest, OrderData>
{
    public Order? Map(OrderData? source)
    {
        return Map(source, new Order());
    }

    public Order? Map(OrderData? source, Order? destination)
    {
        if (source == null)
        {
            return null;
        }

        if (destination == null)
        {
            return Map(source);
        }

        destination.Id = source.Id;
        destination.Date = source.Date;

        return destination;
    }

    public OrderData? Map(OrderRequest? source)
    {
        return Map(source, new OrderData());
    }

    public OrderData? Map(OrderRequest? source, OrderData? destination)
    {
        if (source == null)
        {
            return null;
        }

        if (destination == null)
        {
            return Map(source);
        }
        
        destination.Date = source.Date ?? DateTime.UtcNow;

        return destination;
    }
}
```
### The services
```csharp
public interface IOrderService : IApplicationService
{
    
}

[MapCrudEndpoints]
public class OrderService : IOrderService
{
    private readonly IMapperFactory _mapperFactory;
    private readonly ILogger<ExampleService> _logger;
    private readonly IAsyncQueryableRepository<IQueryableUnitOfWork, OrderData, int> _repository;
    
    public OrderService(
        IApplicationContext<int> applicationContext,
        IQueryableUnitOfWork unitOfWork, 
        IMapperFactory mapperFactory, 
        ILogger<OrderService> logger)
    {
        _mapperFactory = mapperFactory;
        _logger = logger;
        _repository = unitOfWork.GetAsyncQueryableRepository<IQueryableUnitOfWork, OrderData, int>();
    }
    
    public async Task<Order?> GetByIdAsync(int orderId, CancellationToken cancellationToken = default)
    {
        var item = await _repository.GetAsync(orderId, cancellationToken: cancellationToken);

        if (item == null)
        {
            var ex = new EntityNotFoundException<int>(orderId);
            _logger.LogError(ex, "{ServiceName} - GetById: Entity of {EntityName} not found", GetType().Name,
                nameof(OrderData));
            throw ex;
        }

        var mapper = _mapperFactory.GetMapper<OrderData, Order>()!;
        var result = mapper.Map(item);
        
        return result;
    }
}
```

### The `Program.cs`

```csharp
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
    .AddEndpointsApiExplorer()
    .AddApiDocumentation(opt => opt.WithTitle("Example API"));

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseApiDocumentation();
}

app.UseHttpsRedirection();
app.UseRouting();
app.MapControllers();
app.MapGet("orders/{id}", GetById);

app.Run();
return;

async Task<Results<Ok<Order>, NotFound>> GetById(
    [FromRoute] int id, 
    [FromServices] IOrderService service, 
    CancellationToken cancellationToken)
{
    var item = await service.GetByIdAsync(id, cancellationToken);
    if (item == null)
        return TypedResults.NotFound();

    return TypedResults.Ok(item);
}

```