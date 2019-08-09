// Copyright (c) The Avalonia Project. All rights reserved.
// Licensed under the MIT license. See licence.md file in the project root for full license information.

using System;
using Avalonia.Data;

namespace Avalonia
{
    /// <summary>
    /// Interface for getting/setting <see cref="DependencyProperty"/> values on an object.
    /// </summary>
    public interface IAvaloniaObject
    {
        /// <summary>
        /// Raised when a <see cref="DependencyProperty"/> value changes on this object.
        /// </summary>
        event EventHandler<DependencyPropertyChangedEventArgs> PropertyChanged;

        /// <summary>
        /// Raised when an inheritable <see cref="DependencyProperty"/> value changes on this object.
        /// </summary>
        event EventHandler<DependencyPropertyChangedEventArgs> InheritablePropertyChanged;

        /// <summary>
        /// Gets a <see cref="DependencyProperty"/> value.
        /// </summary>
        /// <param name="property">The property.</param>
        /// <returns>The value.</returns>
        object GetValue(DependencyProperty property);

        /// <summary>
        /// Gets a <see cref="DependencyProperty"/> value.
        /// </summary>
        /// <typeparam name="T">The type of the property.</typeparam>
        /// <param name="property">The property.</param>
        /// <returns>The value.</returns>
        T GetValue<T>(DependencyProperty<T> property);

        /// <summary>
        /// Checks whether a <see cref="DependencyProperty"/> is animating.
        /// </summary>
        /// <param name="property">The property.</param>
        /// <returns>True if the property is animating, otherwise false.</returns>
        bool IsAnimating(DependencyProperty property);

        /// <summary>
        /// Checks whether a <see cref="DependencyProperty"/> is set on this object.
        /// </summary>
        /// <param name="property">The property.</param>
        /// <returns>True if the property is set, otherwise false.</returns>
        bool IsSet(DependencyProperty property);

        /// <summary>
        /// Sets a <see cref="DependencyProperty"/> value.
        /// </summary>
        /// <param name="property">The property.</param>
        /// <param name="value">The value.</param>
        /// <param name="priority">The priority of the value.</param>
        void SetValue(
            DependencyProperty property, 
            object value, 
            BindingPriority priority = BindingPriority.LocalValue);

        /// <summary>
        /// Sets a <see cref="DependencyProperty"/> value.
        /// </summary>
        /// <typeparam name="T">The type of the property.</typeparam>
        /// <param name="property">The property.</param>
        /// <param name="value">The value.</param>
        /// <param name="priority">The priority of the value.</param>
        void SetValue<T>(
            DependencyProperty<T> property,
            T value,
            BindingPriority priority = BindingPriority.LocalValue);

        /// <summary>
        /// Binds a <see cref="DependencyProperty"/> to an observable.
        /// </summary>
        /// <param name="property">The property.</param>
        /// <param name="source">The observable.</param>
        /// <param name="priority">The priority of the binding.</param>
        /// <returns>
        /// A disposable which can be used to terminate the binding.
        /// </returns>
        IDisposable Bind(
            DependencyProperty property,
            IObservable<object> source,
            BindingPriority priority = BindingPriority.LocalValue);

        /// <summary>
        /// Binds a <see cref="DependencyProperty"/> to an observable.
        /// </summary>
        /// <typeparam name="T">The type of the property.</typeparam>
        /// <param name="property">The property.</param>
        /// <param name="source">The observable.</param>
        /// <param name="priority">The priority of the binding.</param>
        /// <returns>
        /// A disposable which can be used to terminate the binding.
        /// </returns>
        IDisposable Bind<T>(
            DependencyProperty<T> property,
            IObservable<T> source,
            BindingPriority priority = BindingPriority.LocalValue);
    }
}
