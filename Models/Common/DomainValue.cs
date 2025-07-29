using MongoDB.Bson.Serialization.Attributes;

namespace Angels.Utils.MongoDb.Models.Common;

public record DomainValue : AuditableEntity
{
    [BsonIgnoreIfNull, BsonIgnoreIfDefault]
    public string? ParentId { get; set; } = null;

    [BsonIgnoreIfNull, BsonIgnoreIfDefault]
    public string? Value { get; set; } = null;

    [BsonIgnoreIfNull, BsonIgnoreIfDefault]
    public string? Description { get; set; } = null;
}