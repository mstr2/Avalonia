// Copyright (c) The Avalonia Project. All rights reserved.
// Licensed under the MIT license. See licence.md file in the project root for full license information.

using System;
using System.Reactive;
using Avalonia.Collections;
using Avalonia.Controls;
using Avalonia.Data;

namespace Avalonia.Styling.UnitTests
{
    public class TestControlBase : IStyleable
    {
        public TestControlBase()
        {
            Classes = new Classes();
            SubscribeCheckObservable = new TestObservable();
        }

#pragma warning disable CS0067 // Event not used
        public event EventHandler<DependencyPropertyChangedEventArgs> PropertyChanged;
        public event EventHandler<DependencyPropertyChangedEventArgs> InheritablePropertyChanged;
#pragma warning restore CS0067

        public string Name { get; set; }

        public virtual Classes Classes { get; set; }

        public Type StyleKey => GetType();

        public TestObservable SubscribeCheckObservable { get; private set; }

        public ITemplatedControl TemplatedParent
        {
            get;
            set;
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

        public bool IsAnimating(DependencyProperty property)
        {
            throw new NotImplementedException();
        }

        public bool IsSet(DependencyProperty property)
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
    }
}
