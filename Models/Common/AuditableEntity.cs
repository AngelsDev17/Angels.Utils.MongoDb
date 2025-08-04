using Angels.Utils.MongoDb.Enums;

namespace Angels.Utils.MongoDb.Models.Common;

public abstract class AuditableEntity
{
    public Guid? Id { get; set; } = null;
    public Status Status { get; set; }
    public DateTime? Created { get; set; } = null;
    public DateTime? LastModified { get; set; } = null;
}