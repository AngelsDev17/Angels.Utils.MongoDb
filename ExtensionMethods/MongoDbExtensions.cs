using Angels.Utils.MongoDb.Contexts;
using Angels.Utils.MongoDb.Models.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;

namespace Angels.Utils.MongoDb.ExtensionMethods;

public static class MongoDbExtensions
{
    public static void AddMongoDbExtensions(
        this IServiceCollection serviceCollection,
        Action<DatabaseConfiguration>? dbSettings = null)
    {
        // Serialización para Guid

        BsonSerializer.RegisterSerializer(new GuidSerializer(GuidRepresentation.Standard));

        // Database configuration

        if (dbSettings is null)
        {
            throw new ArgumentNullException(nameof(dbSettings));
        }

        var dbSettingsBuilder = new DatabaseConfiguration();

        dbSettings.Invoke(obj: dbSettingsBuilder);

        serviceCollection.AddSingleton(dbSettingsBuilder);
        serviceCollection.AddSingleton<ApplicationDbContext>();
    }

    public static DatabaseConfiguration GetDbSettings(
        this DatabaseConfiguration connectionInfo,
        DatabaseConfiguration dbSettings)
    {
        connectionInfo.ConnectionString = dbSettings.ConnectionString;
        connectionInfo.DatabaseName = dbSettings.DatabaseName;

        return connectionInfo;
    }
}