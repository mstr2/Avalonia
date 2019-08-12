// Copyright (c) The Avalonia Project. All rights reserved.
// Licensed under the MIT license. See licence.md file in the project root for full license information.

using System;

namespace Avalonia
{
    /// <summary>
    /// A typed avalonia property.
    /// </summary>
    /// <typeparam name="TValue">The value type of the property.</typeparam>
    public class DependencyProperty<TValue> : DependencyProperty
    {
        protected DependencyProperty(
            string name,
            Type ownerType,
            IPropertyMetadata metadata,
            Action<IAvaloniaObject, bool> notifying = null)
            : base(name, typeof(TValue), ownerType, metadata, notifying)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DependencyProperty"/> class.
        /// </summary>
        /// <param name="source">The property to copy.</param>
        /// <param name="ownerType">The new owner type.</param>
        /// <param name="metadata">Optional overridden metadata.</param>
        protected DependencyProperty(
            DependencyProperty source,
            Type ownerType,
            IPropertyMetadata metadata)
            : base(source, ownerType, metadata)
        {
        }
    }
}
