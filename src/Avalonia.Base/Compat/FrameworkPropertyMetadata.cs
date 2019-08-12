// Copyright (c) The Avalonia Project. All rights reserved.
// Licensed under the MIT license. See licence.md file in the project root for full license information.

namespace Avalonia.Compat
{
    public class FrameworkPropertyMetadata : PropertyMetadata
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
            : base(propertyChangedCallback, coerceValueCallback)
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
            AffectsMeasure = (flags & FrameworkPropertyMetadataOptions.AffectsMeasure) != 0;
            AffectsArrange = (flags & FrameworkPropertyMetadataOptions.AffectsArrange) != 0;
        }

        public FrameworkPropertyMetadata(
            object defaultValue,
            FrameworkPropertyMetadataOptions flags,
            PropertyChangedCallback propertyChangedCallback)
            : base(defaultValue, propertyChangedCallback)
        {
            AffectsMeasure = (flags & FrameworkPropertyMetadataOptions.AffectsMeasure) != 0;
            AffectsArrange = (flags & FrameworkPropertyMetadataOptions.AffectsArrange) != 0;
        }

        public FrameworkPropertyMetadata(
            object defaultValue,
            FrameworkPropertyMetadataOptions flags,
            PropertyChangedCallback propertyChangedCallback,
            CoerceValueCallback coerceValueCallback)
            : base(defaultValue, propertyChangedCallback, coerceValueCallback)
        {
            AffectsMeasure = (flags & FrameworkPropertyMetadataOptions.AffectsMeasure) != 0;
            AffectsArrange = (flags & FrameworkPropertyMetadataOptions.AffectsArrange) != 0;
        }

        public bool AffectsMeasure { get; private set; }

        public bool AffectsArrange { get; private set; }

        public override void Merge(IPropertyMetadata baseMetadata, DependencyProperty property)
        {
            base.Merge(baseMetadata, property);

            var src = baseMetadata as FrameworkPropertyMetadata;

            if (src != null)
            {
                if (!AffectsMeasure)
                {
                    AffectsMeasure = src.AffectsMeasure;
                }

                if (!AffectsArrange)
                {
                    AffectsArrange = src.AffectsArrange;
                }
            }
        }
    }
}
