// Copyright (c) The Avalonia Project. All rights reserved.
// Licensed under the MIT license. See licence.md file in the project root for full license information.

using System;
using Avalonia.Interactivity;
using Avalonia.Data;

namespace Avalonia.Controls.Primitives
{
    public class ToggleButton : Button
    {
        public static readonly DirectProperty<ToggleButton, bool?> IsCheckedProperty =
            DependencyProperty.RegisterDirect<ToggleButton, bool?>(
                nameof(IsChecked),
                o => o.IsChecked,
                (o, v) => o.IsChecked = v,
                unsetValue: null,
                defaultBindingMode: BindingMode.TwoWay);

        public static readonly StyledProperty<bool> IsThreeStateProperty =
            DependencyProperty.Register<ToggleButton, bool>(nameof(IsThreeState));

        private bool? _isChecked = false;

        static ToggleButton()
        {
            PseudoClass<ToggleButton, bool?>(IsCheckedProperty, c => c == true, ":checked");
            PseudoClass<ToggleButton, bool?>(IsCheckedProperty, c => c == false, ":unchecked");
            PseudoClass<ToggleButton, bool?>(IsCheckedProperty, c => c == null, ":indeterminate");
        }

        public bool? IsChecked
        {
            get { return _isChecked; }
            set { SetAndRaise(IsCheckedProperty, ref _isChecked, value); }
        }

        public bool IsThreeState
        {
            get => GetValue(IsThreeStateProperty);
            set => SetValue(IsThreeStateProperty, value);
        }

        protected override void OnClick()
        {
            Toggle();
            base.OnClick();
        }

        protected virtual void Toggle()
        {
            if (IsChecked.HasValue)
                if (IsChecked.Value)
                    if (IsThreeState)
                        IsChecked = null;
                    else
                        IsChecked = false;
                else
                    IsChecked = true;
            else
                IsChecked = false;
        }
    }
}
