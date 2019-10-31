using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Runtime.CompilerServices;
using Avalonia.Animation.Easings;
using Avalonia.Collections;
using Avalonia.Controls;

namespace Avalonia.Animation
{
    public class EntranceTransition : IItemContainerTransition
    {
        /// <summary>
        /// Gets the duration of the animation.
        /// </summary> 
        public TimeSpan Duration { get; set; }

        /// <summary>
        /// Gets the easing class to be used.
        /// </summary>
        public Easing Easing { get; set; } = new LinearEasing();
        
        public IDisposable Apply(ItemsControl control, IClock clock, object oldValue, object newValue)
        {
            var oldControls = (AvaloniaList<Control>)oldValue;
            var newControls = (AvaloniaList<Control>)newValue;

            var addedControls = newControls.Except<Control>(oldControls, ReferenceEqualityComparer.Default);
            var removedControls = oldControls.Except<Control>(newControls, ReferenceEqualityComparer.Default);

            TransitionInstance transitionInstance = new TransitionInstance(clock, Duration);

            return transitionInstance.Subscribe(nextValue => {
                foreach (Control added in addedControls)
                {
                    
                }

                foreach (Control removed in removedControls)
                {

                }
            });
        }

        private class ReferenceEqualityComparer : IEqualityComparer, IEqualityComparer<object>
        {
            public static ReferenceEqualityComparer Default { get; } = new ReferenceEqualityComparer();

            public new bool Equals(object x, object y) => ReferenceEquals(x, y);

            public int GetHashCode(object obj) => RuntimeHelpers.GetHashCode(obj);
        }
    }
}
