// Copyright (c) The Avalonia Project. All rights reserved.
// Licensed under the MIT license. See licence.md file in the project root for full license information.

using System;
using Avalonia.Data;

namespace Avalonia
{
    /// <summary>
    /// Base class for avalonia property metadata.
    /// </summary>
    public class PropertyMetadata
    {
        private BindingMode _defaultBindingMode;

        /// <summary>
        /// Initializes a new instance of the <see cref="PropertyMetadata"/> class.
        /// </summary>
        /// <param name="defaultBindingMode">The default binding mode.</param>
        /// <param name="enableDataValidation">
        /// Whether the property is interested in data validation.
        /// </param>
        public PropertyMetadata(
            BindingMode defaultBindingMode = BindingMode.Default,
            bool enableDataValidation = false)
        {
            _defaultBindingMode = defaultBindingMode;
            EnableDataValidation = enableDataValidation;
        }

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
        /// Gets a value indicating whether the property is interested in data validation.
        /// </summary>
        /// <remarks>
        /// Data validation is validation performed at the target of a binding, for example in a
        /// view model using the INotifyDataErrorInfo interface. Only certain properties on a
        /// control (such as a TextBox's Text property) will be interested in recieving data
        /// validation messages so this feature must be explicitly enabled by setting this flag.
        /// </remarks>
        public bool EnableDataValidation { get; }

        /// <summary>
        /// Merges the metadata with the base metadata.
        /// </summary>
        /// <param name="baseMetadata">The base metadata to merge.</param>
        /// <param name="property">The property to which the metadata is being applied.</param>
        public virtual void Merge(
            PropertyMetadata baseMetadata, 
            AvaloniaProperty property)
        {
            if (_defaultBindingMode == BindingMode.Default)
            {
                _defaultBindingMode = baseMetadata.DefaultBindingMode;
            }
        }
    }
}
