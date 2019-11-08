using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.Input;
using System.Collections.Generic;
using Avalonia.Threading;

namespace ControlCatalog
{
    public class DecoratedWindow : Window
    {
        public DecoratedWindow()
        {
            this.InitializeComponent();
            this.AttachDevTools();
        }

        void SetupSide(string name, StandardCursorType cursor, WindowRegion edge)
        {
            var ctl = this.FindControl<Control>(name);
            ctl.Cursor = new Cursor(cursor);
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);

            var regions = new Dictionary<Control, WindowRegion>
            {
                { this.FindControl<Control>("TitleBar"), WindowRegion.Title },
                { this.FindControl<Control>("TopLeft"), WindowRegion.TopLeft },
                { this.FindControl<Control>("Top"), WindowRegion.Top },
                { this.FindControl<Control>("TopRight"), WindowRegion.TopRight },
                { this.FindControl<Control>("Left"), WindowRegion.Left },
                { this.FindControl<Control>("Right"), WindowRegion.Right },
                { this.FindControl<Control>("BottomLeft"), WindowRegion.BottomLeft },
                { this.FindControl<Control>("Bottom"), WindowRegion.Bottom },
                { this.FindControl<Control>("BottomRight"), WindowRegion.BottomRight }
            };

            PlatformImpl?.SetWindowRegionClassifier(point =>
            {
                var a = Dispatcher.UIThread.CheckAccess();
                if (this.InputHitTest(point) is Control control && regions.TryGetValue(control, out var region))
                {
                    return region;
                }

                return WindowRegion.Client;
            });

            SetupSide("Left", StandardCursorType.LeftSide, WindowRegion.Left);
            SetupSide("Right", StandardCursorType.RightSide, WindowRegion.Right);
            SetupSide("Top", StandardCursorType.TopSide, WindowRegion.Top);
            SetupSide("Bottom", StandardCursorType.BottomSide, WindowRegion.Bottom);
            SetupSide("TopLeft", StandardCursorType.TopLeftCorner, WindowRegion.TopLeft);
            SetupSide("TopRight", StandardCursorType.TopRightCorner, WindowRegion.TopRight);
            SetupSide("BottomLeft", StandardCursorType.BottomLeftCorner, WindowRegion.BottomLeft);
            SetupSide("BottomRight", StandardCursorType.BottomRightCorner, WindowRegion.BottomRight);
            this.FindControl<Button>("MinimizeButton").Click += delegate { this.WindowState = WindowState.Minimized; };
            this.FindControl<Button>("MaximizeButton").Click += delegate
            {
                WindowState = WindowState == WindowState.Maximized ? WindowState.Normal : WindowState.Maximized;
            };
            this.FindControl<Button>("CloseButton").Click += delegate
            {
                Close();
            };
        }
    }
}
