namespace Angels.Utils.MongoDb.Models.Configuration;

public record DatabaseConfiguration
{
    public string ConnectionString { get; set; } = string.Empty;
    public string DatabaseName { get; set; } = string.Empty;
    public string? DomainListConnectionString { get; set; } = null;
    public string? DomainListDatabaseName { get; set; } = null;

    public string GetConnectionString(bool isDomainList) => isDomainList ? DomainListConnectionString ?? ConnectionString : ConnectionString;
    public string GetDatabaseName(bool isDomainList) => isDomainList ? DomainListDatabaseName ?? DatabaseName : DatabaseName;
}