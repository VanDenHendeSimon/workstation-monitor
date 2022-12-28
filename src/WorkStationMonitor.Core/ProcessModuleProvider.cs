using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;

namespace WorkStationMonitor.Core;

public interface IProcessModuleProvider
{
    ProcessModule? GetActiveProcessModule();
}

public class ProcessModuleProvider : IProcessModuleProvider
{
    [DllImport("user32.dll")]
    private static extern IntPtr GetWindowThreadProcessId(IntPtr hWnd, out uint processId);

    [DllImport("user32.dll")]
    private static extern IntPtr GetForegroundWindow();

    [ExcludeFromCodeCoverage]
    public ProcessModule? GetActiveProcessModule()
    {
        GetWindowThreadProcessId(
            hWnd: GetForegroundWindow(),
            processId: out var processId);

        return Process
            .GetProcessById((int)processId)
            .MainModule;
    }
}
