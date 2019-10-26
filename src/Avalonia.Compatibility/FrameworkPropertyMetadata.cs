using Avalonia.Data;

namespace Avalonia
{
    public class FrameworkPropertyMetadata : UIPropertyMetadata
    {
        public FrameworkPropertyMetadata()
            : base()
        {
        }

        public FrameworkPropertyMetadata(object defaultValue)
            : base(defaultValue)
        {
        }

        public FrameworkPropertyMetadata(PropertyChangedCallback propertyChangedCallback)
            : base(propertyChangedCallback)
        {
        }

        public FrameworkPropertyMetadata(
            PropertyChangedCallback propertyChangedCallback, CoerceValueCallback coerceValueCallback)
            : base(null, propertyChangedCallback, coerceValueCallback)
        {
        }

        public FrameworkPropertyMetadata(
            object defaultValue, PropertyChangedCallback propertyChangedCallback)
            : base(defaultValue, propertyChangedCallback)
        {
        }

        public FrameworkPropertyMetadata(
            object defaultValue,
            PropertyChangedCallback propertyChangedCallback,
            CoerceValueCallback coerceValueCallback)
            : base(defaultValue, propertyChangedCallback, coerceValueCallback)
        {
        }

        public FrameworkPropertyMetadata(object defaultValue, FrameworkPropertyMetadataOptions flags)
            : base(defaultValue)
        {
            Initialize(flags);
        }

        public FrameworkPropertyMetadata(
            object defaultValue,
            FrameworkPropertyMetadataOptions flags,
            PropertyChangedCallback propertyChangedCallback)
            : base(defaultValue, propertyChangedCallback)
        {
            Initialize(flags);
        }

        public FrameworkPropertyMetadata(
            object defaultValue,
            FrameworkPropertyMetadataOptions flags,
            PropertyChangedCallback propertyChangedCallback,
            CoerceValueCallback coerceValueCallback)
            : base(defaultValue, propertyChangedCallback, coerceValueCallback)
        {
            Initialize(flags);
        }

        public bool AffectsMeasure { get; private set; }

        public bool AffectsArrange { get; private set; }

        public bool AffectsParentMeasure { get; private set; }

        public bool AffectsParentArrange { get; private set; }

        public bool Inherits { get; private set; }

        public bool BindsTwoWayByDefault => DefaultBindingMode == BindingMode.TwoWay;

        public override void Merge(AvaloniaPropertyMetadata baseMetadata, AvaloniaProperty property)
        {
            base.Merge(baseMetadata, property);

            if (baseMetadata is FrameworkPropertyMetadata src)
            {
                if (!Inherits)
                {
                    Inherits = src.Inherits;
                }

                if (!AffectsMeasure)
                {
                    AffectsMeasure = src.AffectsMeasure;
                }

                if (!AffectsArrange)
                {
                    AffectsArrange = src.AffectsArrange;
                }

                if (!AffectsParentMeasure)
                {
                    AffectsParentMeasure = src.AffectsParentMeasure;
                }

                if (!AffectsParentArrange)
                {
                    AffectsParentArrange = src.AffectsParentArrange;
                }
            }
        }

        private void Initialize(FrameworkPropertyMetadataOptions flags)
        {
            Inherits = (flags & FrameworkPropertyMetadataOptions.Inherits) != 0;
            AffectsMeasure = (flags & FrameworkPropertyMetadataOptions.AffectsMeasure) != 0;
            AffectsArrange = (flags & FrameworkPropertyMetadataOptions.AffectsArrange) != 0;
            AffectsParentMeasure = (flags & FrameworkPropertyMetadataOptions.AffectsParentMeasure) != 0;
            AffectsParentArrange = (flags & FrameworkPropertyMetadataOptions.AffectsParentArrange) != 0;

            if ((flags & FrameworkPropertyMetadataOptions.BindsTwoWayByDefault) != 0)
            {
                DefaultBindingMode = BindingMode.TwoWay;
            }
        }
    }
}
