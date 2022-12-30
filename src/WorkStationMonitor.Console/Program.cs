using WorkStationMonitor.Core;
using WorkStationMonitor.Storage;

var applicationService = new ApplicationService(new ProcessModuleProvider());
using var storageService = new StorageService();

var cts = new CancellationTokenSource(TimeSpan.FromHours(10));

while (!cts.IsCancellationRequested)
{
    await Task.Delay(TimeSpan.FromMilliseconds(100));

    var activeApplication = applicationService.GetActiveApplication();
    if (activeApplication is null) continue;

    await storageService.WriteApplicationAsync(activeApplication);
    Console.WriteLine($"{activeApplication.Name}");
}
