// Copyright (c) The Avalonia Project. All rights reserved.
// Licensed under the MIT license. See licence.md file in the project root for full license information.

using System;
using System.Reactive;
using Avalonia.Collections;
using Avalonia.Controls;
using Avalonia.Data;

namespace Avalonia.Styling.UnitTests
{
    public abstract class TestTemplatedControl : ITemplatedControl, IStyleable
    {
        public event EventHandler<DependencyPropertyChangedEventArgs> PropertyChanged;
        public event EventHandler<DependencyPropertyChangedEventArgs> InheritablePropertyChanged;

        public abstract Classes Classes
        {
            get;
        }

        public abstract string Name
        {
            get;
        }

        public abstract Type StyleKey
        {
            get;
        }

        public abstract ITemplatedControl TemplatedParent
        {
            get;
        }

        IAvaloniaReadOnlyList<string> IStyleable.Classes => Classes;

        IObservable<IStyleable> IStyleable.StyleDetach { get; }

        public object GetValue(DependencyProperty property)
        {
            throw new NotImplementedException();
        }

        public T GetValue<T>(DependencyProperty<T> property)
        {
            throw new NotImplementedException();
        }

        public void SetValue(DependencyProperty property, object value, BindingPriority priority)
        {
            throw new NotImplementedException();
        }

        public void SetValue<T>(DependencyProperty<T> property, T value, BindingPriority priority = BindingPriority.LocalValue)
        {
            throw new NotImplementedException();
        }

        public IDisposable Bind(DependencyProperty property, IObservable<object> source, BindingPriority priority = BindingPriority.LocalValue)
        {
            throw new NotImplementedException();
        }

        public IDisposable Bind<T>(DependencyProperty<T> property, IObservable<T> source, BindingPriority priority = BindingPriority.LocalValue)
        {
            throw new NotImplementedException();
        }

        public bool IsAnimating(DependencyProperty property)
        {
            throw new NotImplementedException();
        }

        public bool IsSet(DependencyProperty property)
        {
            throw new NotImplementedException();
        }
    }
}
