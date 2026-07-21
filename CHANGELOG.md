# Changelog

## [3.0.0](https://github.com/eQuantic/core-api/compare/v2.1.0...v3.0.0) (2026-07-21)

### ⚠ BREAKING CHANGES

* repository resolution now uses the two-argument v5 contracts
(IAsyncQueryableRepository<TEntity, TKey>); consumers on the three-argument v4
form must drop the TUnitOfWork type argument.

### Features

* migrate to eQuantic.Core.Data v5 ([e58b42d](https://github.com/eQuantic/core-api/commit/e58b42d35d9b9f5ab072cd60e1c7fef67da217f0))

## [2.1.0](https://github.com/eQuantic/core-api/compare/v2.0.0...v2.1.0) (2026-07-18)

### Features

* adopt the eQuantic.Linq family query collections ([213e5b7](https://github.com/eQuantic/core-api/commit/213e5b7818471e14310800ce3888c07cb7e51cc5))

## [2.0.0](https://github.com/eQuantic/core-api/compare/v1.9.3...v2.0.0) (2026-07-18)

### ⚠ BREAKING CHANGES

* .NET 6 and .NET 9 support is dropped (end of life).
Filtering and sorting now use the eQuantic.Linq v3 engine:
PagedListRequest<TEntity>.FilterBy/OrderBy bind filterBy/orderBy query
values into the new typed FilteringCollection<TEntity> (serializable
ExpressionModel items, AND-combined) and SortingCollection<TEntity>
(QuerySort items) — the eQuantic.Linq v2 IFiltering/ISorting types and the
eQuantic.Core.Mvc model binders are gone (binding works natively in MVC and
Minimal APIs via TryParse/BindAsync).
IWithReferenceId<TDataEntity, TKey>.GetReferenceFiltering() is replaced by
GetReferenceFilter() returning Expression<Func<TDataEntity, bool>>.
AddApiDocumentation now documents the v3 filter grammar and entity member
paths (eQuantic.Linq.Web.Swashbuckle + PagedListRequestOperationFilter,
Swashbuckle 10 on both targets); the ExpressionOperationFilter and
ExpressionSchemaFilter are removed.

### Features

* eQuantic.Core.Api v2 ([e38a871](https://github.com/eQuantic/core-api/commit/e38a8718797617ccf1f9084c5fc1adc00e680572))
