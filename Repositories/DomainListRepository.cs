using Angels.Utils.MongoDb.Contexts;
using Angels.Utils.MongoDb.Enums;
using Angels.Utils.MongoDb.Interfaces;
using Angels.Utils.MongoDb.Models.Common;
using Angels.Utils.MongoDb.Models.Configuration;
using Microsoft.Extensions.Logging;

namespace Angels.Utils.MongoDb.Repositories;

public abstract class DomainListRepository<T>(
    ILogger<CoreRepository<T>> logger,
    ApplicationDbContext applicationDbContext,
    CollectionConfiguration? collectionConfiguration = null) : BaseRepository<T>(
        logger: logger,
        applicationDbContext: applicationDbContext,
        isDomainList: true,
        collectionConfiguration: collectionConfiguration), IDomainListRepository where T : DomainValue
{
    public Task<T> FindByIdAndStatusAsync(Guid id, Status status = Status.Active)
    {
        var filterDefinition = _filterDefinitionBuilder.Eq(item => item.Id, id) &
                               _filterDefinitionBuilder.Eq(item => item.Status, status);

        return FindOne(filterDefinition: filterDefinition);
    }

    public Task<IEnumerable<T>> FindAllByStatusAsync(Status status = Status.Active)
    {
        var filterDefinition = _filterDefinitionBuilder.Eq(item => item.Status, status);
        return FindMany(filterDefinition: filterDefinition);
    }
}