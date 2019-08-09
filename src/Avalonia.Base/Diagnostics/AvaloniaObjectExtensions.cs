// Copyright (c) The Avalonia Project. All rights reserved.
// Licensed under the MIT license. See licence.md file in the project root for full license information.

using Avalonia.Data;

namespace Avalonia.Diagnostics
{
    /// <summary>
    /// Defines diagnostic extensions on <see cref="AvaloniaObject"/>s.
    /// </summary>
    public static class AvaloniaObjectExtensions
    {
        /// <summary>
        /// Gets a diagnostic for a <see cref="DependencyProperty"/> on a <see cref="AvaloniaObject"/>.
        /// </summary>
        /// <param name="o">The object.</param>
        /// <param name="property">The property.</param>
        /// <returns>
        /// A <see cref="AvaloniaPropertyValue"/> that can be used to diagnose the state of the
        /// property on the object.
        /// </returns>
        public static AvaloniaPropertyValue GetDiagnostic(this AvaloniaObject o, DependencyProperty property)
        {
            var set = o.GetSetValues();

            if (set.TryGetValue(property, out var obj))
            {
                if (obj is PriorityValue value)
                {
                    return new AvaloniaPropertyValue(
                        property,
                        o.GetValue(property),
                        (BindingPriority)value.ValuePriority,
                        value.GetDiagnostic());
                }
                else
                {
                    return new AvaloniaPropertyValue(
                        property,
                        obj,
                        BindingPriority.LocalValue,
                        "Local value");
                }
            }
            else
            {
                return new AvaloniaPropertyValue(
                    property,
                    o.GetValue(property),
                    BindingPriority.Unset,
                    "Unset");
            }
        }
    }
}
