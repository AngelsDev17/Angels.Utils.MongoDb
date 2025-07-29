using MongoDB.Bson.Serialization.Attributes;

namespace Angels.Utils.MongoDb.Models.Common;

public record DomainReference
{
    [BsonIgnoreIfNull, BsonIgnoreIfDefault]
    public string? Id { get; set; } = null;

    [BsonIgnoreIfNull, BsonIgnoreIfDefault]
    public string? ParentId { get; set; } = null;

    [BsonIgnoreIfNull, BsonIgnoreIfDefault]
    public string? Value { get; set; } = null;

    [BsonIgnoreIfNull, BsonIgnoreIfDefault]
    public string? Description { get; set; } = null;
}