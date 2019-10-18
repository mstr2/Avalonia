using System;

namespace Avalonia
{
    public sealed class DependencyPropertyKey : IAvaloniaProperty, IAvaloniaPropertyKey
    {
        public DependencyProperty DependencyProperty { get; private set; }

        AvaloniaProperty IAvaloniaPropertyKey.Property => DependencyProperty;

        internal DependencyPropertyKey(DependencyProperty dp)
        {
            DependencyProperty = dp;
        }

        public void OverrideMetadata(Type forType, PropertyMetadata typeMetadata)
        {
            if (DependencyProperty == null)
            {
                throw new InvalidOperationException();
            }

            DependencyProperty.OverrideMetadata(forType, typeMetadata, this);
        }
    }
}
