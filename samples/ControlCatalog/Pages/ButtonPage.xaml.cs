using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.Platform;

namespace ControlCatalog.Pages
{
    public class ButtonPage : UserControl
    {
        public ButtonPage()
        {
            this.InitializeComponent();

            this.Find<ButtonBar>("WindowsButtonBar").Order = ButtonBar.GetDefaultOrder(OperatingSystemType.WinNT);
            this.Find<ButtonBar>("MacOSButtonBar").Order = ButtonBar.GetDefaultOrder(OperatingSystemType.OSX);
            this.Find<ButtonBar>("LinuxButtonBar").Order = ButtonBar.GetDefaultOrder(OperatingSystemType.Linux);
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
