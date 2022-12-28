using InfluxDB.Client;
using InfluxDB.Client.Api.Domain;
using InfluxDB.Client.Writes;
using WorkStationMonitor.Core;

var applicationService = new ApplicationService(new ProcessModuleProvider());

var cts = new CancellationTokenSource(TimeSpan.FromHours(10));
using var client = new InfluxDBClient(
    url: $"http://localhost:{Environment.GetEnvironmentVariable("DOCKER_INFLUXDB_INIT_PORT")}",
    token: Environment.GetEnvironmentVariable("DOCKER_INFLUXDB_INIT_ADMIN_TOKEN"));

while (!cts.IsCancellationRequested)
{
    await Task.Delay(TimeSpan.FromMilliseconds(100));

    var activeApplication = applicationService.GetActiveApplication();
    if (activeApplication is null) continue;

    var writeApi = client.GetWriteApiAsync();
    var point = PointData.Measurement("application")
        .Tag("application_name", activeApplication.Name)
        .Field("v", activeApplication.Name)
        .Timestamp(DateTime.UtcNow, WritePrecision.Ms);

    await writeApi.WritePointAsync(
        point: point,
        bucket: Environment.GetEnvironmentVariable("DOCKER_INFLUXDB_APPLICATION_BUCKET"),
        org: Environment.GetEnvironmentVariable("DOCKER_INFLUXDB_INIT_ORG"));

    Console.WriteLine($"{activeApplication.Name}");
}
