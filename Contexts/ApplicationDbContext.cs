using Angels.Utils.MongoDb.Models.Configuration;
using MongoDB.Driver;

namespace Angels.Utils.MongoDb.Contexts;

public class ApplicationDbContext(DatabaseConfiguration dbSettings)
{
    public IMongoCollection<T> SetMongoCollection<T>(
        bool isDomainList = false,
        CollectionConfiguration? collectionConfiguration = null) where T : class
    {
        var client =
            new MongoClient(
                connectionString: collectionConfiguration?.ConnectionString ??
                                  dbSettings.GetConnectionString(isDomainList: isDomainList));

        var database =
            client.GetDatabase(
                name: collectionConfiguration?.DatabaseName ??
                      dbSettings.GetDatabaseName(isDomainList: isDomainList));

        return database.GetCollection<T>(name: collectionConfiguration?.CollectionName ?? $"{typeof(T).Name}s");
    }
}
