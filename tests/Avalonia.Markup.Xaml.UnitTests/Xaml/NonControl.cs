// Copyright (c) The Avalonia Project. All rights reserved.
// Licensed under the MIT license. See licence.md file in the project root for full license information.

using Avalonia.Controls;

namespace Avalonia.Markup.Xaml.UnitTests.Xaml
{
    public class NonControl : AvaloniaObject
    {
        public static readonly StyledProperty<Control> ControlProperty =
            DependencyProperty.Register<NonControl, Control>(nameof(Control));

        public static readonly StyledProperty<string> StringProperty =
            DependencyProperty.Register<NonControl, string>(nameof(String));

        //No getter or setter Avalonia property
        public static readonly StyledProperty<int> FooProperty =
            DependencyProperty.Register<NonControl, int>("Foo");

        //getter only Avalonia property
        public static readonly StyledProperty<string> BarProperty =
                DependencyProperty.Register<NonControl, string>(nameof(Bar));

        public Control Control
        {
            get { return GetValue(ControlProperty); }
            set { SetValue(ControlProperty, value); }
        }

        public string String
        {
            get { return GetValue(StringProperty); }
            set { SetValue(StringProperty, value); }
        }

        public string Bar
        {
            get { return GetValue(BarProperty); }
        }
    }
}