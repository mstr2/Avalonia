using System;

namespace Avalonia
{
    public delegate object CoerceValueCallback(DependencyObject d, object baseValue);

    public delegate void PropertyChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e);

    public class PropertyMetadata : StyledPropertyMetadata<object>
    {
        public PropertyMetadata()
        {
        }

        public PropertyMetadata(object defaultValue) : base(defaultValue)
        {
        }

        public PropertyMetadata(PropertyChangedCallback propertyChangedCallback)
        {
            PropertyChangedCallback = propertyChangedCallback;
        }

        public PropertyMetadata(
            object defaultValue,
            PropertyChangedCallback propertyChangedCallback)
            : base(defaultValue)
        {
            PropertyChangedCallback = propertyChangedCallback;
        }

        public PropertyMetadata(
            object defaultValue,
            PropertyChangedCallback propertyChangedCallback,
            CoerceValueCallback coerceValueCallback)
            : base(defaultValue, (o, v) => coerceValueCallback((DependencyObject)o, v))
        {
            PropertyChangedCallback = propertyChangedCallback;
            CoerceValueCallback = coerceValueCallback;
        }

        public PropertyChangedCallback PropertyChangedCallback { get; private set; }

        public CoerceValueCallback CoerceValueCallback { get; private set; }

        public override void Merge(AvaloniaPropertyMetadata baseMetadata, AvaloniaProperty property)
        {
            base.Merge(baseMetadata, property);

            var propertyMetadata = baseMetadata as PropertyMetadata;
            if (propertyMetadata != null)
            {
                if (propertyMetadata.PropertyChangedCallback != null)
                {
                    Delegate[] handlers = propertyMetadata.PropertyChangedCallback.GetInvocationList();
                    if (handlers.Length > 0)
                    {
                        PropertyChangedCallback headHandler = (PropertyChangedCallback)handlers[0];
                        for (int i = 1; i < handlers.Length; i++)
                        {
                            headHandler += (PropertyChangedCallback)handlers[i];
                        }

                        headHandler += PropertyChangedCallback;
                        PropertyChangedCallback = headHandler;
                    }
                }

                if (CoerceValueCallback == null)
                {
                    CoerceValueCallback = propertyMetadata.CoerceValueCallback;
                }
            }
        }
    }
}
