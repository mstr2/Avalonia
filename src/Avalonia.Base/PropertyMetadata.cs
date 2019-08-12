// Copyright (c) The Avalonia Project. All rights reserved.
// Licensed under the MIT license. See licence.md file in the project root for full license information.

using System;
using Avalonia.Data;

namespace Avalonia
{
    public delegate void PropertyChangedCallback(IAvaloniaObject d, DependencyPropertyChangedEventArgs e);

    public delegate object CoerceValueCallback(IAvaloniaObject d, object baseValue);

    /// <summary>
    /// Metadata for untyped dependency properties.
    /// </summary>
    public class PropertyMetadata : IPropertyMetadata
    {
        [Flags]
        private enum Flags
        {
            DefaultValueModified,
            CoerceValueCallbackModified
        }

        private BindingMode _defaultBindingMode = BindingMode.Default;
        private Flags _flags;

        public PropertyMetadata()
        {
        }

        public PropertyMetadata(object defaultValue)
        {
            DefaultValue = defaultValue;
            SetFlag(Flags.DefaultValueModified);
        }

        public PropertyMetadata(PropertyChangedCallback propertyChangedCallback)
        {
            PropertyChangedCallback = propertyChangedCallback;
        }

        public PropertyMetadata(object defaultValue, PropertyChangedCallback propertyChangedCallback)
        {
            DefaultValue = defaultValue;
            PropertyChangedCallback = propertyChangedCallback;
            SetFlag(Flags.DefaultValueModified);
        }

        public PropertyMetadata(
            PropertyChangedCallback propertyChangedCallback,
            CoerceValueCallback coerceValueCallback)
        {
            PropertyChangedCallback = propertyChangedCallback;
            CoerceValueCallback = coerceValueCallback;
            SetFlag(Flags.CoerceValueCallbackModified);
        }

        public PropertyMetadata(
            object defaultValue,
            PropertyChangedCallback propertyChangedCallback,
            CoerceValueCallback coerceValueCallback)
        {
            DefaultValue = defaultValue;
            PropertyChangedCallback = propertyChangedCallback;
            CoerceValueCallback = coerceValueCallback;
            SetFlag(Flags.DefaultValueModified | Flags.CoerceValueCallbackModified);
        }

        public object DefaultValue { get; private set; }

        public PropertyChangedCallback PropertyChangedCallback { get; private set; }

        public CoerceValueCallback CoerceValueCallback { get; private set; }

        /// <summary>
        /// Gets the default binding mode for the property.
        /// </summary>
        public BindingMode DefaultBindingMode
        {
            get
            {
                return _defaultBindingMode == BindingMode.Default ?
                    BindingMode.OneWay : _defaultBindingMode;
            }
        }

        /// <summary>
        /// Merges the metadata with the base metadata.
        /// </summary>
        /// <param name="baseMetadata">The base metadata to merge.</param>
        /// <param name="property">The property to which the metadata is being applied.</param>
        public virtual void Merge(
            IPropertyMetadata baseMetadata,
            DependencyProperty property)
        {
            if (_defaultBindingMode == BindingMode.Default)
            {
                _defaultBindingMode = baseMetadata.DefaultBindingMode;
            }

            var src = baseMetadata as PropertyMetadata;

            if (src != null)
            {
                if (HasFlag(Flags.DefaultValueModified))
                {
                    DefaultValue = src.DefaultValue;
                }

                if (HasFlag(Flags.CoerceValueCallbackModified))
                {
                    CoerceValueCallback = src.CoerceValueCallback;
                }

                Delegate[] handlers = src.PropertyChangedCallback.GetInvocationList();
                if (handlers.Length > 0)
                {
                    PropertyChangedCallback head = (PropertyChangedCallback)handlers[0];
                    for (int i = 1; i < handlers.Length; i++)
                    {
                        head += (PropertyChangedCallback)handlers[i];
                    }

                    head += PropertyChangedCallback;
                    PropertyChangedCallback = head;
                }
            }
        }

        private bool HasFlag(Flags flag)
        {
            return (_flags & flag) != 0;
        }

        private void SetFlag(Flags flag)
        {
            _flags &= flag;
        }
    }
}
