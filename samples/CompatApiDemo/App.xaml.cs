using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;

namespace CompatApiDemo
{
    public class App : Application
    {
        public override void Initialize()
        {
            AvaloniaXamlLoader.Load(this);
        }

        public override void OnFrameworkInitializationCompleted()
        {
            ((IClassicDesktopStyleApplicationLifetime)ApplicationLifetime).MainWindow = new MainWindow();
            base.OnFrameworkInitializationCompleted();
        }
    }

}
