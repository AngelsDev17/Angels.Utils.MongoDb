using Angels.Utils.MongoDb.Contexts;
using Angels.Utils.MongoDb.Enums;
using Angels.Utils.MongoDb.Models.Common;
using Angels.Utils.MongoDb.Models.Configuration;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;

namespace Angels.Utils.MongoDb.Repositories;

public abstract class CoreRepository<T>(
    ILogger<CoreRepository<T>> logger,
    ApplicationDbContext applicationDbContext,
    bool isDomainList,
    CollectionConfiguration? collectionConfiguration = null) where T : AuditableEntity
{
    private readonly ILogger _logger = logger;

    protected readonly IMongoCollection<T> _mongoCollection = applicationDbContext.SetMongoCollection<T>(
        isDomainList: isDomainList,
        collectionConfiguration: collectionConfiguration);

    protected readonly FilterDefinitionBuilder<T> _filterDefinitionBuilder = Builders<T>.Filter;
    protected readonly UpdateDefinitionBuilder<T> _updateDefinitionBuilder = Builders<T>.Update;

    protected async Task<T> FindOneById(Guid id)
    {
        var filterDefinition = _filterDefinitionBuilder.Eq(item => item.Id, id);
        return await FindOne(filterDefinition: filterDefinition);
    }
    protected async Task<T> FindOneByStatus(Status status = Status.Active)
    {
        var filterDefinition = _filterDefinitionBuilder.Eq(item => item.Status, status);
        return await FindOne(filterDefinition: filterDefinition);
    }
    protected async Task<T> FindOneByIdAndStatus(Guid id, Status status = Status.Active)
    {
        var filterDefinition = _filterDefinitionBuilder.Eq(item => item.Id, id) &
                               _filterDefinitionBuilder.Eq(item => item.Status, status);

        return await FindOne(filterDefinition: filterDefinition);
    }
    protected async Task<T> FindFirstRecord()
    {
        try
        {
            return await _mongoCollection
                .Find(_filterDefinitionBuilder.Where(item => true))
                .Limit(1)
                .Sort(Builders<T>.Sort.Ascending(field: nameof(AuditableEntity.Created)))
                .FirstOrDefaultAsync();
        }
        catch (Exception e)
        {
            _logger.LogError(
                message: "Error al buscar el primer documento de tipo {type}.",
                exception: new MongoException(message: e.Message),
                args: typeof(T).Name);

            throw;
        }
    }
    protected async Task<T> FindLastRecord()
    {
        try
        {
            return await _mongoCollection
                .Find(_filterDefinitionBuilder.Where(item => true))
                .Limit(1)
                .Sort(Builders<T>.Sort.Descending(field: nameof(AuditableEntity.Created)))
                .FirstOrDefaultAsync();
        }
        catch (Exception e)
        {
            _logger.LogError(
                message: "Error al buscar el ultimo documento de tipo {type}.",
                exception: new MongoException(message: e.Message),
                args: typeof(T).Name);

            throw;
        }
    }
    protected async Task<T> FindOne(FilterDefinition<T> filterDefinition)
    {
        try
        {
            return await (await _mongoCollection.FindAsync(filter: filterDefinition)).FirstOrDefaultAsync();
        }
        catch (Exception e)
        {
            _logger.LogError(
                message: "Error al buscar el documento de tipo {type}.",
                exception: new MongoException(message: e.Message),
                args: typeof(T).Name);

            throw;
        }
    }

    protected async Task<IEnumerable<T>> FindMany()
    {
        var filterDefinition = _filterDefinitionBuilder.Where(item => true);
        return await FindMany(filterDefinition: filterDefinition);
    }
    protected async Task<IEnumerable<T>> FindManyByStatus(Status status = Status.Active)
    {
        var filterDefinition = _filterDefinitionBuilder.Eq(item => item.Status, status);
        return await FindMany(filterDefinition: filterDefinition);
    }
    protected async Task<IEnumerable<T>> FindMany(FilterDefinition<T> filterDefinition)
    {
        try
        {
            return await (await _mongoCollection.FindAsync(filter: filterDefinition)).ToListAsync();
        }
        catch (Exception e)
        {
            _logger.LogError(
                message: "Error al buscar los documentos de tipo {type}.",
                exception: new MongoException(message: e.Message),
                args: typeof(T).Name);

            throw;
        }
    }

    protected async Task<long> Count(FilterDefinition<T> filterDefinition)
    {
        try
        {
            return await _mongoCollection.CountDocumentsAsync(filter: filterDefinition);
        }
        catch (Exception e)
        {
            _logger.LogError(
                message: "Error al contar los documentos de tipo {type}.",
                exception: new MongoException(message: e.Message),
                args: typeof(T).Name);

            throw;
        }
    }

    protected async Task<Guid> InsertOne(T entity)
    {
        Guid id;

        try
        {
            id = SetBasicPropertiesToEntity(entity: ref entity);
            await _mongoCollection.InsertOneAsync(document: entity);
        }
        catch (Exception e)
        {
            _logger.LogError(
                message: "Error al insertar el documento de tipo {type}.",
                exception: new MongoException(message: e.Message),
                args: typeof(T).Name);

            throw;
        }

        return id;
    }
    protected async Task<IEnumerable<Guid>> InsertMany(IEnumerable<T> entities)
    {
        if (!entities.Any())
        {
            _logger.LogWarning(
                message: "No se han proporcionado entidades para insertar de tipo {type}.",
                args: typeof(T).Name);

            return [];
        }

        var ids = new List<Guid>();
        var entitiesList = entities as IList<T> ?? entities.ToList();

        try
        {
            for (var i = 0; i < entitiesList.Count; i++)
            {
                var entity = entitiesList.ElementAt(i);
                ids.Add(SetBasicPropertiesToEntity(entity: ref entity));
                entitiesList[i] = entity;
            }

            await _mongoCollection.InsertManyAsync(documents: entitiesList);
        }
        catch (Exception e)
        {
            _logger.LogError(
                message: "Error al insertar los documentos de tipo {type}.",
                exception: new MongoException(message: e.Message),
                args: typeof(T).Name);

            throw;
        }

        return ids;
    }

    protected async Task UpdateStatusById(Guid id, Status newStatus)
    {
        var filterDefinition = _filterDefinitionBuilder.Eq(item => item.Id, id);
        var updateDefinition = _updateDefinitionBuilder.Set(item => item.Status, newStatus);

        await UpdateOne(
            filterDefinition: filterDefinition,
            updateDefinition: updateDefinition);
    }
    protected async Task UpdateStatusByCurrentStatus(Status currentStatus, Status newStatus)
    {
        var filterDefinition = _filterDefinitionBuilder.Eq(item => item.Status, currentStatus);
        var updateDefinition = _updateDefinitionBuilder.Set(item => item.Status, newStatus);

        await UpdateMany(
            filterDefinition: filterDefinition,
            updateDefinition: updateDefinition);
    }
    protected async Task UpdateStatusByIdAndCurrentStatus(Guid id, Status currentStatus, Status newStatus)
    {
        var filterDefinition = _filterDefinitionBuilder.Eq(item => item.Id, id) &
                               _filterDefinitionBuilder.Eq(item => item.Status, currentStatus);

        var updateDefinition = _updateDefinitionBuilder.Set(item => item.Status, newStatus);

        await UpdateOne(
            filterDefinition: filterDefinition,
            updateDefinition: updateDefinition);
    }
    protected async Task UpdateOne(FilterDefinition<T> filterDefinition, UpdateDefinition<T> updateDefinition)
    {
        var properties = typeof(T).GetProperties();

        try
        {
            var lastModified = properties.FirstOrDefault(item => item.Name.Equals(nameof(AuditableEntity.LastModified)));

            if (lastModified is not null)
            {
                updateDefinition = _updateDefinitionBuilder.Combine(
                                       updateDefinition,
                                       _updateDefinitionBuilder.Set(
                                           field: nameof(AuditableEntity.LastModified),
                                           value: DateTime.UtcNow));
            }

            await _mongoCollection.UpdateOneAsync(
                filter: filterDefinition,
                update: updateDefinition);
        }
        catch (Exception e)
        {
            _logger.LogError(
                message: "Error al actualizar el documento de tipo {type}.",
                exception: new MongoException(message: e.Message),
                args: typeof(T).Name);

            throw;
        }
    }
    protected async Task UpdateMany(FilterDefinition<T> filterDefinition, UpdateDefinition<T> updateDefinition)
    {
        var properties = typeof(T).GetProperties();

        try
        {
            var lastModified = properties.FirstOrDefault(item => item.Name.Equals(nameof(AuditableEntity.LastModified)));

            if (lastModified is not null)
            {
                updateDefinition = _updateDefinitionBuilder.Combine(
                    updateDefinition,
                    _updateDefinitionBuilder.Set(
                        field: nameof(AuditableEntity.LastModified),
                        value: DateTime.UtcNow));
            }

            await _mongoCollection.UpdateManyAsync(
                filter: filterDefinition,
                update: updateDefinition);
        }
        catch (Exception e)
        {
            _logger.LogError(
                message: "Error al actualizar los documentos de tipo {type}.",
                exception: new MongoException(message: e.Message),
                args: typeof(T).Name);

            throw;
        }
    }

    protected async Task DeleteOne(Guid id)
    {
        var filterDefinition = _filterDefinitionBuilder.Eq(item => item.Id, id);
        await DeleteOne(filterDefinition: filterDefinition);
    }
    protected async Task DeleteMany()
    {
        var filterDefinition = _filterDefinitionBuilder.Where(item => true);
        await DeleteMany(filterDefinition: filterDefinition);
    }
    protected async Task DeleteOne(FilterDefinition<T> filterDefinition)
    {
        try
        {
            await _mongoCollection.DeleteOneAsync(filter: filterDefinition);
        }
        catch (Exception e)
        {
            _logger.LogError(
                message: "Error al eliminar el documento de tipo {type}.",
                exception: new MongoException(message: e.Message),
                args: typeof(T).Name);

            throw;
        }
    }
    protected async Task DeleteMany(FilterDefinition<T> filterDefinition)
    {
        try
        {
            await _mongoCollection.DeleteManyAsync(filter: filterDefinition);
        }
        catch (Exception ex)
        {
            _logger.LogError(
                message: "Error al eliminar los documentos de tipo {type}.",
                exception: new MongoException(message: ex.Message),
                args: typeof(T).Name);

            throw;
        }
    }

    private static Guid SetBasicPropertiesToEntity(ref T entity)
    {
        var id = Guid.Empty;

        foreach (var property in typeof(T).GetProperties())
        {
            switch (property.Name)
            {
                case nameof(AuditableEntity.Id):
                    var currentId = property.GetValue(obj: entity);

                    if (currentId is null)
                    {
                        id = Guid.NewGuid();
                        property.SetValue(obj: entity, value: id);
                    }
                    else
                    {
                        id = (Guid)currentId;
                    }
                    break;

                case nameof(AuditableEntity.Created) or nameof(AuditableEntity.LastModified):
                    property.SetValue(obj: entity, value: DateTime.UtcNow);
                    break;
            }
        }

        return id;
    }
}