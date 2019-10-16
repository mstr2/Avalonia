using Avalonia;

namespace CompatApiDemo
{
    class Program
    {
        static void Main(string[] args)
        {
            AppBuilder.
                Configure<App>().
                UsePlatformDetect().
                UseSkia().
                With(new Win32PlatformOptions
                {
                    AllowEglInitialization = true
                }).
                StartWithClassicDesktopLifetime(args);
        }
    }
}
