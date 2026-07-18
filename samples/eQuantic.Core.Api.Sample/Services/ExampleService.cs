using eQuantic.Core.Api.Sample.Entities;
using eQuantic.Core.Api.Sample.Entities.Data;
using eQuantic.Core.Application;
using eQuantic.Core.Application.Services;
using eQuantic.Core.Data.Repository;
using eQuantic.Core.Exceptions;
using eQuantic.Mapper;

namespace eQuantic.Core.Api.Sample.Services;

public interface IExampleService : IApplicationService
{
    Task<Example?> GetByIdAsync(int exampleId, CancellationToken cancellationToken = default);
}

public class ExampleService : IExampleService
{
    private readonly IMapperFactory _mapperFactory;
    private readonly ILogger<ExampleService> _logger;
    private readonly IAsyncQueryableRepository<IQueryableUnitOfWork, ExampleData, int> _repository;
    
    public ExampleService(
        IApplicationContext<int> applicationContext,
        IQueryableUnitOfWork unitOfWork, 
        IMapperFactory mapperFactory, 
        ILogger<ExampleService> logger)
    {
        _mapperFactory = mapperFactory;
        _logger = logger;
        _repository = unitOfWork.GetAsyncQueryableRepository<IQueryableUnitOfWork, ExampleData, int>();
    }
    
    public async Task<Example?> GetByIdAsync(int exampleId, CancellationToken cancellationToken = default)
    {
        var item = await _repository.GetAsync(exampleId, cancellationToken: cancellationToken);

        if (item == null)
        {
            var ex = new EntityNotFoundException<int>(exampleId);
            _logger.LogError(ex, "{ServiceName} - GetById: Entity of {EntityName} not found", GetType().Name,
                nameof(ExampleData));
            throw ex;
        }

        var mapper = _mapperFactory.GetMapper<ExampleData, Example>()!;
        var result = mapper.Map(item);
        
        return result;
    }
}