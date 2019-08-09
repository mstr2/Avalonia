﻿using System;

namespace Avalonia.Reactive
{
    internal class AvaloniaPropertyChangedObservable : 
        LightweightObservableBase<DependencyPropertyChangedEventArgs>,
        IDescription
    {
        private readonly WeakReference<IAvaloniaObject> _target;
        private readonly DependencyProperty _property;

        public AvaloniaPropertyChangedObservable(
            IAvaloniaObject target,
            DependencyProperty property)
        {
            _target = new WeakReference<IAvaloniaObject>(target);
            _property = property;
        }

        public string Description => $"{_target.GetType().Name}.{_property.Name}";

        protected override void Initialize()
        {
            if (_target.TryGetTarget(out var target))
            {
                target.PropertyChanged += PropertyChanged;
            }
        }

        protected override void Deinitialize()
        {
            if (_target.TryGetTarget(out var target))
            {
                target.PropertyChanged -= PropertyChanged;
            }
        }

        private void PropertyChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (e.Property == _property)
            {
                PublishNext(e);
            }
        }
    }
}
