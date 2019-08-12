// Copyright (c) The Avalonia Project. All rights reserved.
// Licensed under the MIT license. See licence.md file in the project root for full license information.

using System;
using Avalonia.Compat;

namespace Avalonia
{
    public delegate bool ValidateValueCallback(object value);

    /// <summary>
    /// A property class that is similar to WPF-style dependency properties.
    /// </summary>
    internal class UntyedProperty : DependencyProperty, IPropertyAccessor
    {
        private readonly bool _inherits;
        private readonly Func<IAvaloniaObject, object, object> _validation;

        /// <summary>
        /// Initializes a new instance of the <see cref="UntyedProperty"/> class.
        /// </summary>
        /// <param name="name">The name of the property.</param>
        /// <param name="propertyType">The type of the property.</param>
        /// <param name="ownerType">The type of the class that registers the property.</param>
        /// <param name="metadata">The property metadata.</param>
        /// <param name="validateValueCallback">The validation function.</param>
        /// <param name="inherits">Whether the property inherits its value.</param>
        /// <param name="notifying">A <see cref="DependencyProperty.Notifying"/> callback.</param>
        internal UntyedProperty(
            string name,
            Type propertyType,
            Type ownerType,            
            PropertyMetadata metadata,
            ValidateValueCallback validateValueCallback,
            bool inherits = false,
            Action<IAvaloniaObject, bool> notifying = null)
                : base(name, propertyType, ownerType, metadata, notifying)
        {
            Contract.Requires<ArgumentNullException>(name != null);
            Contract.Requires<ArgumentNullException>(ownerType != null);

            if (name.Contains("."))
            {
                throw new ArgumentException($"'{nameof(name)}' may not contain periods.");
            }

            _inherits = inherits;

            if (validateValueCallback != null)
            {
                _validation = (obj, value) => validateValueCallback(value) ? value : obj.GetValue(this);
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="StyledPropertyBase{T}"/> class.
        /// </summary>
        /// <param name="source">The property to add the owner to.</param>
        /// <param name="ownerType">The type of the class that registers the property.</param>
        protected UntyedProperty(UntyedProperty source, Type ownerType)
            : base(source, ownerType, null)
        {
            _inherits = source.Inherits;
        }

        /// <summary>
        /// Gets a value indicating whether the property inherits its value.
        /// </summary>
        /// <value>
        /// A value indicating whether the property inherits its value.
        /// </value>
        public override bool Inherits => _inherits;

        /// <summary>
        /// Gets the default value for the property on the specified type.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns>The default value.</returns>
        public object GetDefaultValue(Type type)
        {
            Contract.Requires<ArgumentNullException>(type != null);

            return ((PropertyMetadata)GetMetadata(type)).DefaultValue;
        }
        
        /// <summary>
        /// Overrides the metadata for the property on the specified type.
        /// </summary>
        /// <typeparam name="T">The type.</typeparam>
        /// <param name="metadata">The metadata.</param>
        public void OverrideMetadata<T>(PropertyMetadata metadata) where T : IAvaloniaObject
        {
            base.OverrideMetadata(typeof(T), metadata);
        }

        /// <summary>
        /// Overrides the metadata for the property on the specified type.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <param name="metadata">The metadata.</param>
        public void OverrideMetadata(Type type, PropertyMetadata metadata)
        {
            base.OverrideMetadata(type, metadata);
        }

        /// <summary>
        /// Gets the string representation of the property.
        /// </summary>
        /// <returns>The property's string representation.</returns>
        public override string ToString()
        {
            return Name;
        }

        internal override void NotifyChanged(DependencyPropertyChangedEventArgs e)
        {
            var metadata = (PropertyMetadata)GetMetadata(GetType());

            IFrameworkInvalidatable frameworkInvalidatable = e.Sender as IFrameworkInvalidatable;
            if (frameworkInvalidatable != null)
            {
                var frameworkMetadata = metadata as FrameworkPropertyMetadata;
                if (frameworkMetadata != null)
                {
                    if (frameworkMetadata.AffectsMeasure)
                    {
                        frameworkInvalidatable.InvalidateMeasure();
                    }

                    if (frameworkMetadata.AffectsArrange)
                    {
                        frameworkInvalidatable.InvalidateArrange();
                    }
                }
            }

            metadata.PropertyChangedCallback?.Invoke(e.Sender, e);

            base.NotifyChanged(e);
        }

        /// <inheritdoc/>
        Func<IAvaloniaObject, object, object> IPropertyAccessor.GetValidationFunc(Type type)
        {
            Contract.Requires<ArgumentNullException>(type != null);
            return _validation;
        }
    }
}
