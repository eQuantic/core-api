# Changelog

## [5.0.0](https://github.com/eQuantic/core-api/compare/v4.0.0...v5.0.0) (2026-11-14)

### ⚠ BREAKING CHANGES

* core-api now depends on core-ddd 6.0, which crosses two majors: EntityBase<TKey> compares by identity instead of by reference (5.0), and the repositories come from eQuantic.Core.Data 7.0, which changes what some existing calls do, such as a duplicate key surfacing as DuplicateKeyException at commit (6.0). Applications get both through core-api.

### Features

* move to core-ddd 6.0 and publish through trusted publishing ([#4](https://github.com/eQuantic/core-api/issues/4)) ([c3cffcf](https://github.com/eQuantic/core-api/commit/c3cffcf1bcdbaae50661eb6e73a3d8cf35f96215))

## [4.0.0](https://github.com/eQuantic/core-api/compare/v3.0.0...v4.0.0) (2026-07-22)

### ⚠ BREAKING CHANGES

* **datamodel:** DataModel now owns IEntity/IEntity<TKey>. Consumers must
upgrade eQuantic.Core.Data and eQuantic.Core.DataModel together — mixing this
DataModel with eQuantic.Core.Data <= 5.8 (which still defines IEntity) yields a
duplicate/ambiguous type (CS0433/CS0311). eQuantic.Core.Data 5.9 drops the
definition and forwards the type. The Api.Sample references core-data 5.8
directly and will bump to 5.9 as part of the coordinated upgrade.

### Features

* **datamodel:** own the IEntity contract and drop the dependency on eQuantic.Core.Data ([0edb843](https://github.com/eQuantic/core-api/commit/0edb843e601c4a7340ba77044f0dc2af0fdcd3e3))

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
