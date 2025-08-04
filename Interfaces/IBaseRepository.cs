using Angels.Utils.MongoDb.Enums;

namespace Angels.Utils.MongoDb.Interfaces;

public interface IBaseRepository<T>
{
    Task<T> FindOneById(Guid id);
    Task<T> FindOneByStatus(Status status = Status.Active);
    Task<T> FindOneByIdAndStatus(Guid id, Status status = Status.Active);
    Task<T> FindFirstRecord();
    Task<T> FindLastRecord();

    Task<IEnumerable<T>> FindMany();
    Task<IEnumerable<T>> FindManyByStatus(Status status = Status.Active);

    Task<Guid> InsertOne(T entity);
    Task<IEnumerable<Guid>> InsertMany(IEnumerable<T> entities);

    Task UpdateStatusById(Guid id, Status newStatus);
    Task UpdateStatusByCurrentStatus(Status currentStatus, Status newStatus);
    Task UpdateStatusByIdAndCurrentStatus(Guid id, Status currentStatus, Status newStatus);

    Task DeleteOneById(Guid id);
    Task DeleteMany();
}