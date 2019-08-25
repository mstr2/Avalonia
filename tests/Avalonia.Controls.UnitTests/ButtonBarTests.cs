using System;
using Xunit;

namespace Avalonia.Controls.UnitTests
{
    public class ButtonBarTests
    {
        [Fact]
        public void Buttons_Are_Windows_Ordered()
        {
            var buttonBar = new ButtonBar();
            buttonBar.Order = ButtonBar.GetDefaultOrder(Avalonia.Platform.OperatingSystemType.WinNT);

            CheckOrder(buttonBar);
        }

        [Fact]
        public void Buttons_Are_macOS_Ordered()
        {
            var buttonBar = new ButtonBar();
            buttonBar.Order = ButtonBar.GetDefaultOrder(Avalonia.Platform.OperatingSystemType.OSX);

            CheckOrder(buttonBar);
        }

        [Fact]
        public void Buttons_Are_Linux_Ordered()
        {
            var buttonBar = new ButtonBar();
            buttonBar.Order = ButtonBar.GetDefaultOrder(Avalonia.Platform.OperatingSystemType.Linux);

            CheckOrder(buttonBar);
        }

        private void CheckOrder(ButtonBar buttonBar)
        {
            foreach (ButtonKind kind in Enum.GetValues(typeof(ButtonKind)))
            {
                var button = new Button();
                ButtonBar.SetKind(button, kind);
                buttonBar.Children.Add(button);
            }

            for (int i = 0; i < buttonBar.Children.Count; ++i)
            {
                Assert.Equal(buttonBar.Order[i], ButtonBar.GetKind(buttonBar.OrderedChildren[i]));
            }
        }
    }
}
