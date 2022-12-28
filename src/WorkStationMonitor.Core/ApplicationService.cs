using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.InteropServices;
using WorkStationMonitor.Core.Extensions;

namespace WorkStationMonitor.Core;
public class ApplicationService
{
    private readonly IProcessModuleProvider _processModuleProvider;

    public ApplicationService(IProcessModuleProvider processModuleProvider)
    {
        _processModuleProvider = processModuleProvider;
    }

    public Application? GetActiveApplication()
    {
        try
        {
            var mainModule = _processModuleProvider.GetActiveProcessModule();

            return mainModule?.FileName is null
                ? null
                : new Application(mainModule.FileName);
        }
        catch (Win32Exception) { return null; }
    }
}

public sealed record Application
{
    private string _fileName;

    public Application(string fileName)
    {
        if (string.IsNullOrEmpty(fileName))
            throw new ArgumentException("fileName should not be null nor empty");

        _fileName = fileName;
    }

    public string Name => Path.GetFileNameWithoutExtension(_fileName).Capitalize();
}
