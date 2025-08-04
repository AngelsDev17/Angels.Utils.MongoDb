using Angels.Utils.MongoDb.Contexts;
using Angels.Utils.MongoDb.Enums;
using Angels.Utils.MongoDb.Interfaces;
using Angels.Utils.MongoDb.Models.Common;
using Angels.Utils.MongoDb.Models.Configuration;
using Microsoft.Extensions.Logging;

namespace Angels.Utils.MongoDb.Repositories;

public abstract class BaseRepository<T>(
    ILogger<CoreRepository<T>> logger,
    ApplicationDbContext applicationDbContext,
    bool isDomainList = false,
    CollectionConfiguration? collectionConfiguration = null) : CoreRepository<T>(
        logger: logger,
        applicationDbContext: applicationDbContext,
        isDomainList: isDomainList,
        collectionConfiguration: collectionConfiguration), IBaseRepository<T> where T : AuditableEntity
{
    public new Task<T> FindOneById(Guid id) => base.FindOneById(id: id);
    public new Task<T> FindOneByStatus(Status status = Status.Active) => base.FindOneByStatus(status: status);
    public new Task<T> FindOneByIdAndStatus(Guid id, Status status = Status.Active) => base.FindOneByIdAndStatus(id: id, status: status);
    public new Task<T> FindFirstRecord() => base.FindFirstRecord();
    public new Task<T> FindLastRecord() => base.FindLastRecord();

    public new Task<IEnumerable<T>> FindMany() => base.FindMany();
    public new Task<IEnumerable<T>> FindManyByStatus(Status status = Status.Active) => base.FindManyByStatus(status: status);

    public new Task<Guid> InsertOne(T entity) => base.InsertOne(entity: entity);
    public new Task<IEnumerable<Guid>> InsertMany(IEnumerable<T> entities) => base.InsertMany(entities: entities);

    public new Task UpdateStatusById(Guid id, Status newStatus) => base.UpdateStatusById(id: id, newStatus: newStatus);
    public new Task UpdateStatusByCurrentStatus(Status currentStatus, Status newStatus) => base.UpdateStatusByCurrentStatus(currentStatus: currentStatus, newStatus: newStatus);
    public new Task UpdateStatusByIdAndCurrentStatus(Guid id, Status currentStatus, Status newStatus) => base.UpdateStatusByIdAndCurrentStatus(id: id, currentStatus: currentStatus, newStatus: newStatus);

    public new Task DeleteOneById(Guid id) => base.DeleteOneById(id: id);
    public new Task DeleteMany() => base.DeleteMany();
}