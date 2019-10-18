using System;

namespace Avalonia
{
    public delegate object CoerceValueCallback(AvaloniaObject d, object baseValue);

    public delegate void PropertyChangedCallback(AvaloniaObject d, AvaloniaPropertyChangedEventArgs e);

    public class PropertyMetadata : StyledPropertyMetadata<object>
    {
        private readonly bool _defaultValueModified;

        public PropertyMetadata()
        {
        }

        public PropertyMetadata(object defaultValue) : base(defaultValue)
        {
            _defaultValueModified = true;
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
            _defaultValueModified = true;
            PropertyChangedCallback = propertyChangedCallback;
        }

        public PropertyMetadata(
            object defaultValue,
            PropertyChangedCallback propertyChangedCallback,
            CoerceValueCallback coerceValueCallback)
            : base(defaultValue, (o, v) => coerceValueCallback((AvaloniaObject)o, v))
        {
            _defaultValueModified = true;
            PropertyChangedCallback = propertyChangedCallback;
            CoerceValueCallback = coerceValueCallback;
        }

        public PropertyChangedCallback PropertyChangedCallback { get; private set; }

        public CoerceValueCallback CoerceValueCallback { get; private set; }

        public override void Merge(AvaloniaPropertyMetadata baseMetadata, AvaloniaProperty property)
        {
            var currentDefaultValue = DefaultValue;
            
            base.Merge(baseMetadata, property);

            if (baseMetadata is StyledPropertyMetadata<object> styledPropertyMetadata)
            {
                DefaultValue = _defaultValueModified ? currentDefaultValue : styledPropertyMetadata.DefaultValue;
            }

            if (baseMetadata is PropertyMetadata propertyMetadata)
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
