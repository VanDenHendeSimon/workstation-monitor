using InfluxDB.Client;
using InfluxDB.Client.Api.Domain;
using InfluxDB.Client.Writes;
using WorkStationMonitor.Core;

namespace WorkStationMonitor.Storage;

public class StorageService : IDisposable
{
    private readonly InfluxDBClient _client;

    public StorageService()
    {
        _client = new InfluxDBClient(
            url: $"http://localhost:{Environment.GetEnvironmentVariable("DOCKER_INFLUXDB_INIT_PORT")}",
            token: Environment.GetEnvironmentVariable("DOCKER_INFLUXDB_INIT_ADMIN_TOKEN"));
    }

    public Task WriteApplicationAsync(Application application)
    {
        var point = PointData.Measurement("application")
            .Tag("application_name", application.Name)
            .Field("v", application.Name)
            .Timestamp(DateTime.UtcNow, WritePrecision.Ms);

        var writeApi = _client.GetWriteApiAsync();
        return writeApi.WritePointAsync(
            point: point,
            bucket: Environment.GetEnvironmentVariable("DOCKER_INFLUXDB_APPLICATION_BUCKET"),
            org: Environment.GetEnvironmentVariable("DOCKER_INFLUXDB_INIT_ORG"));
    }

    public void Dispose() => _client.Dispose();
}
