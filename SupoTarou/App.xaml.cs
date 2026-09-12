using System.Runtime.InteropServices;
using System.Windows;

namespace SupoTarou;

/// <summary>
/// Interaction logic for App.xaml
/// </summary>
public partial class App : Application
{
    private static readonly nint DpiAwarenessContextPerMonitorAwareV2 = new(-4);

    protected override void OnStartup(StartupEventArgs e)
    {
        ConfigureDpiAwareness();
        base.OnStartup(e);
    }

    private static void ConfigureDpiAwareness()
    {
        if (!SetProcessDpiAwarenessContext(DpiAwarenessContextPerMonitorAwareV2))
        {
            SetProcessDPIAware();
        }
    }

    [DllImport("user32.dll", SetLastError = true)]
    private static extern bool SetProcessDpiAwarenessContext(nint dpiContext);

    [DllImport("user32.dll")]
    private static extern bool SetProcessDPIAware();
}
