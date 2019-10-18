namespace Avalonia
{
    public class UIPropertyMetadata : PropertyMetadata
    {
        public UIPropertyMetadata() : base()
        {
        }

        public UIPropertyMetadata(object defaultValue) : base(defaultValue)
        {
        }

        public UIPropertyMetadata(PropertyChangedCallback propertyChangedCallback)
            : base(propertyChangedCallback)
        {
        }

        public UIPropertyMetadata(
            object defaultValue,
            PropertyChangedCallback propertyChangedCallback)
            : base(defaultValue, propertyChangedCallback)
        {
        }

        public UIPropertyMetadata(
            object defaultValue,
            PropertyChangedCallback propertyChangedCallback,
            CoerceValueCallback coerceValueCallback)
            : base(defaultValue, propertyChangedCallback, coerceValueCallback)
        {
        }
    }
}
