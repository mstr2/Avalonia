using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace CompatApiDemo
{
    public class MainWindow : Window
    {
        public MainWindow()
        {
            DataContext = new MainViewModel();
            this.InitializeComponent();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
