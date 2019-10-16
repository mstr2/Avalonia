using System;
using Avalonia;

namespace CompatApiDemo
{
    public class MainViewModel : DependencyObject
    {
        public static readonly DependencyProperty ValueProperty = DependencyProperty.Register(
            "Value", typeof(double), typeof(MainViewModel), new PropertyMetadata(0.0, null, CoerceValue));

        private static object CoerceValue(DependencyObject d, object baseValue)
        {
            return Math.Max(5.0, Math.Min(10.0, (double)baseValue));
        }

        public double Value
        {
            get { return (double)GetValue(ValueProperty); }
            set { SetValue(ValueProperty, value); }
        }
    }
}
