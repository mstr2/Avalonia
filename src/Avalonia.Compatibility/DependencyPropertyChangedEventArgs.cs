using System;
using Avalonia.Data;

namespace Avalonia
{
    /// <summary>
    /// Provides information for a dependency property change.
    /// </summary>
    public struct DependencyPropertyChangedEventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DependencyPropertyChangedEventArgs"/> class.
        /// </summary>
        /// <param name="sender">The object that the property changed on.</param>
        /// <param name="property">The property that changed.</param>
        /// <param name="oldValue">The old value of the property.</param>
        /// <param name="newValue">The new value of the property.</param>
        public DependencyPropertyChangedEventArgs(
            DependencyObject sender,
            DependencyProperty property,
            object oldValue,
            object newValue)
        {
            Sender = sender;
            Property = property;
            OldValue = oldValue;
            NewValue = newValue;
        }

        /// <summary>
        /// Gets the <see cref="DependencyObject"/> that the property changed on.
        /// </summary>
        /// <value>The sender object.</value>
        public DependencyObject Sender { get; private set; }

        /// <summary>
        /// Gets the property that changed.
        /// </summary>
        /// <value>
        /// The property that changed.
        /// </value>
        public DependencyProperty Property { get; private set; }

        /// <summary>
        /// Gets the old value of the property.
        /// </summary>
        /// <value>
        /// The old value of the property.
        /// </value>
        public object OldValue { get; private set; }

        /// <summary>
        /// Gets the new value of the property.
        /// </summary>
        /// <value>
        /// The new value of the property.
        /// </value>
        public object NewValue { get; private set; }
    }
}
